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

var builder = WebApplication.CreateBuilder(args);

string CORSOpenPolicy = "OpenCORSPolicy";
ConectionOptions conectionsOptions = builder.Configuration.GetSection(ConectionOptions.Connection).Get<ConectionOptions>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(
        options => options.UseSqlServer(conectionsOptions.DefaultConection));

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

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress?.ToString());
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestPath", httpContext.Request.Path);
            diagnosticContext.Set("ResponseStatusCode", httpContext.Response.StatusCode);
        };
    });

    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors(CORSOpenPolicy);
app.UseAuthorization();
app.MapControllers();


using (var scope = app.Services.CreateScope())
 {
     var rabbitMQConsumer = scope.ServiceProvider.GetRequiredService<RabbitMQConsumer>();
     await rabbitMQConsumer.StartListeningAsync();
}

app.Run();
