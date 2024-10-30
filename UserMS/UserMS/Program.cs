using UserMS.Core.DomainLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserMS.Logic.ServiceLayer.IServices;
using UserMS.Logic.ServiceLayer.Services;
using UserMS.Data.RepositoryLayer.IRepository;
using UserMS.Data.RepositoryLayer.Repository;
using Microsoft.OpenApi.Models;
using UserMS.Core.DomainLayer.Configuration;
using Serilog;
using Serilog.Templates;
using Serilog.Enrichers;
using UserMS.API.Middleware;
using Serilog.Formatting.Json;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.Json.Serialization;

using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

string CORSOpenPolicy = "OpenCORSPolicy";
ConectionOptions conectionsOptions = builder.Configuration.GetSection(ConectionOptions.Connection).Get<ConectionOptions>();

AuthenticationOptions authenticationOptions = builder.Configuration.GetSection(AuthenticationOptions.Auth).Get<AuthenticationOptions>();

builder.Services.Configure<AuthenticationOptions>(
    builder.Configuration.GetSection(AuthenticationOptions.Auth));

builder.Services.AddDbContext<AppDbContext>(
        options => options.UseSqlServer(conectionsOptions.DefaultConection));

builder.Services.AddSerilog((services, lc) => lc
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
    "logs/" + DateTime.Now.Year + "-" + DateTime.Now.Month + "/" + "log-.log",
    rollingInterval: RollingInterval.Day,
    rollOnFileSizeLimit: true,
    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    ));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpClient<UserService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7155/");
});



builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
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
                                        Type=ReferenceType.SecurityScheme,
                                        Id="Bearer"
                                }
                        },
                        new string[]{}
                }
        });
});

builder.Services.AddCors(options => {
    options.AddPolicy(
        name: CORSOpenPolicy,
        builder => {
            builder.WithOrigins("http://localhost:4200", "http://localhost:8080","http://frontend:8080")
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSerilogRequestLogging(options =>
    {

        options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {

            diagnosticContext.Set("ConnectionRemoteIpAddress", httpContext.Connection.RemoteIpAddress);
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
