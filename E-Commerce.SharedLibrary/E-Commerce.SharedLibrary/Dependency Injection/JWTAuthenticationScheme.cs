﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace E_Commerce.SharedLibrary.Dependency_Injection
{
    public  static class JWTAuthenticationScheme
    {
        public static IServiceCollection AddJWTAuthenticationScheme(this IServiceCollection services , IConfiguration configuration)
        {
               services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Bearer" , options =>
               {
                   var key = Encoding.UTF8.GetBytes(configuration.GetSection("Authentication:key").Value!);
                   string issuer = configuration.GetSection("Authentication:Issuer").Value!;
                   string audience = configuration.GetSection("Authentication:Audience").Value;

                   options.RequireHttpsMetadata = false;
                   options.SaveToken = true;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true ,
                       ValidateAudience = true,
                       ValidateLifetime = false ,
                       ValidateIssuerSigningKey = true ,
                       ValidIssuer = issuer,
                       ValidAudience = audience,
                       IssuerSigningKey = new SymmetricSecurityKey(key)

                   };
               }
               
               );
            return services;
        }
    }
}
