namespace AIMShipping.ClientConsole
{
    using AIM.Shipping.Contracts.Interfaces;
    using AIM.Shipping.Contracts.RnR;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstracted away but also easier to port over to another platform/host if needed
    /// This class is the actual main entry point of the application
    /// </summary>
    public sealed class MainEntryPoint
    {
        private readonly IShippingClient _shippingClient = null;
        private readonly ILogger _logger = null;

        public MainEntryPoint(IShippingClient client, ILogger<MainEntryPoint> logger)
        {
            this._shippingClient = client ?? throw new ArgumentNullException(nameof(client), "No client specified");
            this._logger = logger ?? new Logger<MainEntryPoint>(new LoggerFactory());
        }

        /// <summary>
        /// Our main entry point
        /// </summary>
        /// <param name="args">pass through args if needed</param>
        /// <returns>Task</returns>
        public async Task Run(string[] args)
        {
            // Let's call our shipping service method and get a response and display it on the console
            var response = await this._shippingClient.GetShippingStatusAsync(new GetShippingStatusRequest { });
            if (!response.Success)
            {
                this._logger.LogError($"Service call was not successful. Error message: {response.FailureInformation}");
            }
            else
            {
                this._logger.LogInformation("Service call returned the following information:");
                this._logger.LogInformation($"Ship date: {response.ShippingDataInformation.ShipDate}");
                if (!response.ShippingDataInformation.ShipDate.HasValue)
                {
                    this._logger.LogInformation("There was no shipping data found!!");
                }
            }
        }
    }
}
