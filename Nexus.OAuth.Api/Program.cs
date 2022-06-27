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

namespace Nexus.OAuth.Api;

/// <summary>
/// 
/// </summary>
public static class Program
{
    private const string AllownedOriginsKey = "AllownedOrigins";
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
        var supportedCultures = new[] { "pt-BR", "en-US" };
        var builder = WebApplication.CreateBuilder(args);
        ConfigurationManager configuration = builder.Configuration;

        AuthenticationHelper = new(configuration.GetConnectionString("SqlServer"));

        // Add services to the container.
        builder.Services.AddControllers()
            // Transform enum number in enum name in api result
            .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(Swagger.AddSwagger);
        builder.Services
            .AddRequestLocalization((RequestLocalizationOptions options) =>
                options.SetDefaultCulture(supportedCultures[0])
               .AddSupportedCultures(supportedCultures)
               .AddSupportedUICultures(supportedCultures));

        var app = builder.Build();

        string[] allownedsOrigins = app
            .Configuration
            .GetSection(AllownedOriginsKey)
            .Get<string[]>();

        app.UseRequestLocalization();

        app.UseWebSockets(GetSocketOptions(allownedsOrigins));

        app.UseSwagger();
        app.UseSwaggerUI();

#if !DEBUG
        app.UseHsts();
        app.UseHttpsRedirection();
#endif
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