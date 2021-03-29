using AIM.Common;
using AIM.Shipping.Contracts.Interfaces;
using AIM.Shipping.Contracts.RnR;
using AIMShipping.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AIM.Shipping.Implementation
{
    /// <summary>
    /// Class which acts as a wrapper/self contained to call the necessary services to interact with it.
    /// </summary>
    public sealed class ShippingClient : IShippingClient
    {
        private readonly ShippingServiceClientConfiguration _shippingConfig = null;
        private readonly HttpClient _shippingClient = null;
        private readonly ILogger _logger = null;

        /// <summary>
        /// Constructor which is used to construct the shipping client to interact with external services.
        /// </summary>
        /// <param name="shippingclient">HTTP client factory to call the services</param>
        /// <param name="config">Configuration values to potentially override the HTTP client setup but also anything else this class may need a config for.
        /// Intentionally not using the IConfiguration as we want ensure the class is self contained and gets what it really needs and nothing outside of it's scope.
        /// </param>
        public ShippingClient(ILogger<ShippingClient> logger, HttpClient shippingclient, ShippingServiceClientConfiguration config)
        {
            this._shippingClient = shippingclient ?? throw new ArgumentNullException(nameof(shippingclient), "A non null HTTP client must be supplied");
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger), "A non null logger must be supplied");
            

            this._shippingConfig = config ?? throw new ArgumentNullException(nameof(config), "A non null config must be supplied");

            if (string.IsNullOrWhiteSpace(config.EndpointUrl))
            {
                throw new ArgumentNullException(nameof(config.EndpointUrl), "Endpoint URL must be supplied which calls the service");
            }

            this._shippingClient.BaseAddress = new Uri(config.EndpointUrl);
        }


        /// <summary>
        /// Method which gets the shipping status
        /// </summary>
        /// <param name="request">The request object</param>
        /// <returns>The response containing the shipping data information</returns>
        public async Task<GetShippingStatusResponse> GetShippingStatus(GetShippingStatusRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
