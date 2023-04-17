using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Npgsql;
using PropertyBase.Contracts;
using PropertyBase.Data;
using PropertyBase.DTOs.Authentication;
using PropertyBase.Entities;
using PropertyBase.Exceptions;
using PropertyBase.Extensions;
using PropertyBase.Routes;
using PropertyBase.Services;

var builder = WebApplication.CreateBuilder(args);


//load .env on dev
DotNetEnv.Env.Load();
DotNetEnv.Env.TraversePath().Load();

// Add services to the container.
builder.Services.AddCors(option =>
{
    option.AddPolicy("Public", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Property Base API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Please enter token",
        BearerFormat = "JWT",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


builder.RegisterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Property Base API");
        c.RoutePrefix = string.Empty;
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("Public");
app.UseMiddleware<RequestStatusCodeHandler>();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

// PING API
app.MapGet("/api/ping", () =>
{
    return Results.Ok(new { Message = "PONG!" });
});

app.MapGroup("/api/accounts")
    .UserApi()
    .WithTags("Account")
    .WithOpenApi();

app.MapGroup("/api/agency")
    .AgencyApi()
    .WithTags("Agency")
    .WithOpenApi()
    .RequireAuthorization(AuthorizationPolicy.AgencyPolicy);

app.Run();

