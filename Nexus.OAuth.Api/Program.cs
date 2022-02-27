#region Global Usings
global using System.Net;
global using System.IO;
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
using Nexus.OAuth.Api.Controllers;

var builder = WebApplication.CreateBuilder(args);


// Add ConnectionString in dbContext
#region ConnectionString
OAuthContext.ConnectionString = builder.Configuration
#if LOCAL
    .GetConnectionString("LocalDev");
#elif DEBUG 
    .GetConnectionString("Development");
#else
    .GetConnectionString("Release");
#endif
#endregion

// Add services to the container.

builder.Services.AddControllers()
// Transform enum number in enum name in api result
    .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
    .WithOrigins(
#if DEBUG || LOCAL
    "https://localhost:44337", "localhost:44337", "https://nexus-oauth.duckdns.org"
#else
    "https://web-nexus.duckdns.org", "https://oauth.nexus-company.tech"
#endif
     ).WithHeaders("Client-Key", "Authorization", "X-Code", "X-Validation")
    .AllowAnyMethod()
    .AllowCredentials()
);

// Use Nexus Middleware for control clients authentications
app.UseAuthentication(AuthenticationHelper.ValidAuthenticationResultAsync);

app.UseEndpoints(endpoints =>
    endpoints.MapControllers());

app.Run();