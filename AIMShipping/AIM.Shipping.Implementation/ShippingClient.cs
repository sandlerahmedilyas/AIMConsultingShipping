﻿using AIM.Common;
using AIM.Shipping.Contracts.Interfaces;
using AIM.Shipping.Contracts.RnR;
using AIM.Shipping.Models;
using AIMShipping.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace AIM.Shipping.Implementation
{
    /// <summary>
    /// Class which acts as a wrapper/self contained to call the necessary services to interact with it.
    /// </summary>
    public sealed class ShippingClient : IShippingClient
    {
        private readonly ShippingServiceClientConfiguration _shippingConfig = null;
        private readonly IHttpClientFactory _clientFactory = null;
        private readonly ILogger _logger = null;

        /// <summary>
        /// Constructor which is used to construct the shipping client to interact with external services.
        /// </summary>
        /// <param name="shippingClientFactory">HTTP client factory to call the services</param>
        /// <param name="config">Configuration values to potentially override the HTTP client setup but also anything else this class may need a config for.
        /// Intentionally not using the IConfiguration as we want ensure the class is self contained and gets what it really needs and nothing outside of it's scope.
        /// </param>
        public ShippingClient(ILogger<ShippingClient> logger, IHttpClientFactory shippingClientFactory, ShippingServiceClientConfiguration config)
        {
            this._clientFactory = shippingClientFactory ?? throw new ArgumentNullException(nameof(shippingClientFactory), "A non null HTTP client must be supplied");
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger), "A non null logger must be supplied");            

            this._shippingConfig = config ?? throw new ArgumentNullException(nameof(config), "A non null config must be supplied");

            if (string.IsNullOrWhiteSpace(config.EndpointUrl))
            {
                throw new ArgumentNullException(nameof(config.EndpointUrl), "Endpoint URL must be supplied which calls the service");
            }
        }


        /// <summary>
        /// Method which gets the shipping status
        /// </summary>
        /// <param name="request">The request object</param>
        /// <returns>The response containing the shipping data information</returns>
        public async Task<GetShippingStatusResponse> GetShippingStatusAsync(GetShippingStatusRequest request)
        {
            var response = new GetShippingStatusResponse();

            try
            {
                // always get a new client. Under the hood, the DelegatingHandler is managed by the runtime in .NET core better so no change of port exhaustion or stale DNS entries.
                using (var client = this.GetHTTPClient())
                {
                    using (var httpResp = await client.GetAsync(this._shippingConfig.EndpointUrl))
                    {
                        httpResp.EnsureSuccessStatusCode();

                        // parse the output.
                        // in this case I know what my data model is. Same properties but the data returning from the service as a JSON is a different casing,
                        // so let's specify the formatter properties
                        var serializerOptions = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };
                        var stringData = await httpResp.Content.ReadAsStringAsync();
                        response.ShippingDataInformation = JsonSerializer.Deserialize<ShippingDataModel>(stringData, serializerOptions);
                        response.Success = true;
                    }
                }
            }
            catch(HttpRequestException ex)
            {
                // There is a problem so log it and then return the response object back to the caller with a nice message that there was a problem.
                this._logger.LogError(ex.ToString());
                response.Success = false;
                response.FailureInformation = "There was an error during calling the service. Please try again later";
            }
            catch(Exception ex)
            {
                // There is a problem so log it and then return the response object back to the caller with a nice message that there was a problem.
                this._logger.LogError(ex.ToString());
                response.Success = false;
                response.FailureInformation = "There was an error calling the service. Please try again later";
            }

            return await Task.FromResult(response);
        }

        /// <summary>
        /// Simple method that returns a default settings for http client. Can be used to add default headers etc...
        /// </summary>
        /// <returns>Configured HTTP Client</returns>
        private HttpClient GetHTTPClient()
        {
            var client = this._clientFactory.CreateClient(StringConstants.ShippingHTTP_NamedClient);
            client.BaseAddress = new Uri(this._shippingConfig.EndpointUrl);

            return client;
        }
    }
}
