#region Global Usings
global using System.Net;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Nexus.OAuth.Dal;
global using Nexus.OAuth.Dal.Models;
global using Nexus.OAuth.Dal.Models.Enums;
global using Nexus.OAuth.Server.Models.Result;
global using Nexus.OAuth.Server.Models.Upload;
global using Nexus.Tools.Validations.Middlewares.Authentication.Attributes;
#endregion

using System.Text.Json.Serialization;
using Nexus.OAuth.Server.Controllers;
using Nexus.Tools.Validations.Middlewares.Authentication;


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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

/// Use Nexus Middleware for control clients authentications
app.UseAuthentication(AuthenticationsController.ValidAuthenticationResultAsync);

app.Run();



