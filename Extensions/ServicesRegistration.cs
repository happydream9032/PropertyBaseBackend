using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PropertyBase.Data;
using PropertyBase.DTOs.Authentication;
using PropertyBase.Entities;
using System.Text;
using Newtonsoft.Json;
using Npgsql;
using PropertyBase.Contracts;
using PropertyBase.Data.Repositories;
using PropertyBase.Services;


namespace PropertyBase.Extensions
{
    public static class ServicesRegistration
    {
       public static IServiceCollection RegisterServices(this WebApplicationBuilder builder)
        {
            var connStringBuilder = new NpgsqlConnectionStringBuilder();
            connStringBuilder.Host = DotNetEnv.Env.GetString("DB_HOST");
            connStringBuilder.Port = DotNetEnv.Env.GetInt("DB_PORT");
            connStringBuilder.SslMode = SslMode.VerifyFull;
            connStringBuilder.Username = DotNetEnv.Env.GetString("DB_USER");
            connStringBuilder.Password = DotNetEnv.Env.GetString("DB_PASSWORD");
            connStringBuilder.Database = DotNetEnv.Env.GetString("DB_NAME");
            

            builder.Services.AddDbContext<PropertyBaseDbContext>(options =>
                      options.UseNpgsql(connStringBuilder.ConnectionString));
            
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            builder.Services.AddIdentity<User, IdentityRole>(c =>
            {
                c.User.RequireUniqueEmail = true;
                c.SignIn.RequireConfirmedEmail = true;
                c.Password.RequiredLength = 8;
                c.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            })
                  .AddEntityFrameworkStores<PropertyBaseDbContext>()
                  .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
                                )
                             .AddJwtBearer(o =>
                             {
                                 o.RequireHttpsMetadata = false;
                                 o.SaveToken = false;
                                 o.TokenValidationParameters = new TokenValidationParameters
                                 {
                                     ValidateIssuerSigningKey = true,
                                     ValidateIssuer = true,
                                     ValidateAudience = true,
                                     ValidateLifetime = true,
                                     ClockSkew = TimeSpan.Zero,
                                     ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                                     ValidAudience = builder.Configuration["JwtSettings:Audience"],
                                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
                                 };
                                 o.Events = new JwtBearerEvents
                                 {
                                     OnChallenge = context =>
                                     {
                                         context.HandleResponse();
                                         context.Response.StatusCode = 401;
                                         context.Response.ContentType = "application/json";
                                         var result = JsonConvert.SerializeObject(new { Message = "Not authenticated." });
                                         return context.Response.WriteAsync(result);
                                     },

                                     OnForbidden = context =>
                                     {
                                         context.Response.StatusCode = 403;
                                         context.Response.ContentType = "application/json";
                                         var result = JsonConvert.SerializeObject(new { Message = "Not authorized." });
                                         return context.Response.WriteAsync(result);
                                     }
                                 };
                             });

            builder.Services.AddAuthorization();

            builder.Services.AddHttpClient();

            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IAgencyRepository, AgencyRepository>();
            builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
            builder.Services.AddTransient<IEmailService, EmailService>();

            return builder.Services;
        }
    }
}

