﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.SharedLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderAPi_Application.Services;
using Polly;
using Polly.Retry;

namespace OrderAPi_Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services , IConfiguration configuration)
        {
            // Resgister HttpCilent service
            // Create Department Injection
            services.AddHttpClient<IOrderService , OrderService>(option =>
            {
                option.BaseAddress = new Uri(configuration["ApiGateway:BaseAddress"]!);
                option.Timeout = TimeSpan.FromSeconds(1);
            });

            // Create Retry Strategy
            var retryStrategy = new RetryStrategyOptions()
            { 
            
            ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
            BackoffType = DelayBackoffType.Constant,
            UseJitter = true,
            MaxRetryAttempts = 3,
            Delay=TimeSpan.FromMilliseconds(500),
            OnRetry = args =>
            {
                string message = $"OnRetry , Attempt {args.AttemptNumber} OutCome {args.Outcome}";
                LogException.LogToConsole(message);
                LogException.LogToDebugger(message);
                return ValueTask.CompletedTask;

            }
            
            };
            // Use Retry Stratgy
            services.AddResiliencePipeline("my-retry-pipeline", builder =>
            {
                builder.AddRetry(retryStrategy);
            });

            return services;
        }
    }
}
