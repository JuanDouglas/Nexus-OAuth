#region Global Usings
global using System.Net;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Nexus.OAuth.Dal;
global using Nexus.OAuth.Dal.Models;
global using Nexus.OAuth.Dal.Models.Enums;
global using Nexus.OAuth.Api.Models.Result;
global using Nexus.OAuth.Api.Models.Upload;
global using Nexus.OAuth.Api.Models.Enums;
global using Nexus.OAuth.Domain;
global using Nexus.Tools.Validations.Middlewares.Authentication.Attributes;
#endregion
using System.Text.Json.Serialization;
using Nexus.Tools.Validations.Middlewares.Authentication;
using Nexus.OAuth.Domain.Authentication;
using SixLabors.ImageSharp;
using BenjaminAbt.HCaptcha.AspNetCore;

namespace Nexus.OAuth.Api;

/// <summary>
/// 
/// </summary>
public static class Program
{
    private const string AllownedOriginsKey = "AllownedOrigins";
    public const bool IsDebug =
#if DEBUG
        true;
#else
        false;
#endif
    public const string Environment =
#if LOCAL
        "Local";
#elif DEBUG
        "Debug";
#else
        "Release";
#endif

    public static string[] AllownedHeaders =
    {
        "Content-Type", "Client-Key", "Authorization", "X-Code", "X-Validation", "X-Code-Id"
    };

    public static AuthenticationHelper AuthenticationHelper;

    /// <summary>
    /// Application entry point
    /// </summary>
    /// <param name="args">Start application arguments</param>
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        ConfigurationManager config = builder.Configuration;

        AuthenticationHelper = new(config.GetConnectionString("SqlServer"));

        ConfigureServices(builder.Services, config);

        var app = builder.Build();

        string[] allownedsOrigins = app
            .Configuration
            .GetSection(AllownedOriginsKey)
            .Get<string[]>();

        app.UseRequestLocalization();

        app.UseWebSockets(GetSocketOptions(allownedsOrigins));

        app.UseSwagger();
        app.UseSwaggerUI();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();

        app.UseRouting();
        app.UseCors(builder =>
            builder
                .WithOrigins(allownedsOrigins)
                .WithHeaders(AllownedHeaders)
                .AllowAnyMethod()
                .AllowCredentials());

        // Use Nexus Middleware for control clients authentications
        app.UseAuthentication(AuthenticationHelper.ValidAuthenticationResultAsync);

        app.UseEndpoints(endpoints =>
            endpoints.MapControllers());

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, ConfigurationManager config)
    {
        var supportedCultures = new[] { "pt-BR", "en-US" };

        // Add services to the container.
        services.AddControllers()
             // Transform enum number in enum name in api result
             .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(Swagger.AddSwagger);
        services
            .AddRequestLocalization((RequestLocalizationOptions options) =>
                options.SetDefaultCulture(supportedCultures[0])
               .AddSupportedCultures(supportedCultures)
               .AddSupportedUICultures(supportedCultures));

        services.AddHCaptcha(config.GetSection("HCaptcha"));
    }
    private static WebSocketOptions GetSocketOptions(string[] allowneds)
    {
        var opts = new WebSocketOptions()
        {
            KeepAliveInterval = TimeSpan.FromMilliseconds(3000)
        };

        foreach (var origin in allowneds)
            opts.AllowedOrigins.Add(origin);

        return opts;
    }
}