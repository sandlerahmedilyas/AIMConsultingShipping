namespace AIMShipping.ClientConsole
{
    using AIM.Shipping.Contracts.Interfaces;
    using AIM.Shipping.Implementation;
    using AIMShipping.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.IO;
    using Microsoft.Extensions.Options;
    using System.Net.Http;
    using Polly;
    using Polly.Extensions.Http;

    public static class Startup
    {
        /// <summary>
        /// The usual of configuring our modular services.
        /// </summary>
        /// <returns>Service collection containing all configuration for the app</returns>
        public static IServiceCollection ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            // let's set up our builder and configure it.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())                
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            var configuration = builder.Build();

            // Config is fine as singleton.
            serviceCollection.AddSingleton(configuration);
            
            // Add some logging
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConsole();
            });

            // and options to be passed in if requested
            serviceCollection.AddOptions();
            
            // Configure our configuration using the section in the config file
            serviceCollection.Configure<ShippingServiceClientConfiguration>(configuration.GetSection("ShippingServiceClientConfiguration")); // let's bind the settings to the class and inject it in wherever required

            serviceCollection.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ShippingServiceClientConfiguration>>().Value);

            // Add our http client and add POLLY to handle our backoff/retry mechinism for any http issues in flight/transient.
            serviceCollection.AddHttpClient<IShippingClient, ShippingClient>()
            .AddPolicyHandler(GetRetryPolicy(configuration));

            serviceCollection.AddSingleton<IShippingClient, ShippingClient>();
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddTransient<MainEntryPoint>();

            return serviceCollection;
        }
        
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IConfiguration config)
        {
            var retryValue = 5;
            if (!string.IsNullOrWhiteSpace(config["HttpRetryCount"]))
            {
                _ = Int32.TryParse(config["HttpRetryCount"], out retryValue);
            }

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(retryValue, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
