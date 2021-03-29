using AIM.Shipping.Contracts.Interfaces;
using AIM.Shipping.Implementation;
using AIMShipping.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Microsoft.Extensions.Options;

namespace AIMShipping.ClientConsole
{
    public static class Startup
    {
        /// <summary>
        /// The usual of configuring our modular services.
        /// </summary>
        /// <returns>Service collection containing all configuration for the app</returns>
        public static IServiceCollection ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())                
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            var configuration = builder.Build();

            serviceCollection.AddSingleton(configuration);
            
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConsole();
            });

            serviceCollection.AddOptions();
            serviceCollection.Configure<ShippingServiceClientConfiguration>(configuration.GetSection("ShippingServiceClientConfiguration")); // let's bind the settings to the class and inject it in wherever required

            serviceCollection.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ShippingServiceClientConfiguration>>().Value);

            serviceCollection.AddHttpClient();
            serviceCollection.AddSingleton<IShippingClient, ShippingClient>();
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddTransient<MainEntryPoint>();

            return serviceCollection;
        }
    }
}
