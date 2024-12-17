using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text;
using System.Text.Json.Serialization;
using ChatMS.Configuration;
using ChatMS;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

string CORSOpenPolicy = "OpenCORSPolicy";

AuthenticationOptions authenticationOptions = builder.Configuration
    .GetSection(AuthenticationOptions.Auth)
    .Get<AuthenticationOptions>();
builder.Services.Configure<AuthenticationOptions>(
    builder.Configuration.GetSection(AuthenticationOptions.Auth)
);

builder.Services.AddSerilog((services, lc) => lc
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/log-.log", rollingInterval: RollingInterval.Day)
);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Endpoints", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options => {
    options.AddPolicy(CORSOpenPolicy, builder =>
    {
        builder.WithOrigins("http://localhost:4200", "http://localhost:8080", "http://frontend:8080", "http://localhost")
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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSerilogRequestLogging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseCors(CORSOpenPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chat");


app.UseRouting();

app.Run();
