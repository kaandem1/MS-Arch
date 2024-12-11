using System.Net;
using MCMS.Data;
using MCMS.Services;
using MCMS.RepositoryLayer;
using MCMS.Models;
using MCMS.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.Json.Serialization;
using AuthenticationOptions = MCMS.Configuration.AuthenticationOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);

string CORSOpenPolicy = "OpenCORSPolicy";
ConectionOptions conectionsOptions = builder.Configuration.GetSection(ConectionOptions.Connection).Get<ConectionOptions>();

AuthenticationOptions authenticationOptions = builder.Configuration.GetSection(AuthenticationOptions.Auth).Get<AuthenticationOptions>();

builder.Services.Configure<AuthenticationOptions>(builder.Configuration.GetSection(AuthenticationOptions.Auth));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(conectionsOptions.DefaultConection));

builder.Host.UseSerilog((context, config) =>
{
    config.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
          .ReadFrom.Configuration(context.Configuration)
          .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
          .WriteTo.File("logs/" + DateTime.Now.ToString("yyyy-MM") + "/log-.log",
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true,
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
});

builder.Services.AddScoped<IDeviceConsumptionRepository, DeviceInfoRepository>();
builder.Services.AddScoped<IDeviceService, DeviceService>();

builder.Services.AddScoped<RabbitMQConsumer>();

//builder.Services.AddScoped<WebSocketService>();
builder.Services.AddScoped<WebSocketService>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MCMS API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORSOpenPolicy,
        policy =>
        {
            policy.WithOrigins("http://localhost:8080", "http://localhost")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = authenticationOptions.ValidIssuer,
            ValidAudience = authenticationOptions.ValidAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(authenticationOptions.IssuerSecurityKey)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddHostedService<RabbitMQBackgroundService>();

var app = builder.Build();
app.UseWebSockets();

var webSocketService = app.Services.GetRequiredService<WebSocketService>();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await webSocketService.HandleWebSocketConnection(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress?.ToString());
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        };
    });

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MCMS API v1"));
}

app.UseCors(CORSOpenPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
