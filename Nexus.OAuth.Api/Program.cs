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
    public const string Environment =
#if LOCAL
        "Local";
#elif DEBUG
        "Debug";
#else
        "Release";
#endif

    public static string[] AllownedOrigins =
#if DEBUG || LOCAL
        { "https://localhost:44337", "localhost:44337", "https://nexus-oauth.duckdns.org"};
#else
        { "https://oauth.nexus-company.tech", "https://nexus-oauth.azurewebsites.net"};
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

        app.UseRequestLocalization();

        app.UseWebSockets(GetSocketOptions());

        app.UseSwagger();
        app.UseSwaggerUI();

#if !DEBUG
        app.UseHsts();
#endif

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors(builder =>
            builder
                .WithOrigins(AllownedOrigins)
                .WithHeaders(AllownedHeaders)
                .AllowAnyMethod()
                .AllowCredentials()
        );

        // Use Nexus Middleware for control clients authentications
        app.UseAuthentication(AuthenticationHelper.ValidAuthenticationResultAsync);

        app.UseEndpoints(endpoints =>
            endpoints.MapControllers());

        app.Run();
    }
    private static WebSocketOptions GetSocketOptions()
    {
        var opts = new WebSocketOptions()
        {
            KeepAliveInterval = TimeSpan.FromMilliseconds(3000)
        };

        foreach (var origin in AllownedOrigins)
            opts.AllowedOrigins.Add(origin);

        return opts;
    }
}