namespace AIM.Shipping.Contracts.Interfaces
{
    using AIM.Shipping.Contracts.RnR;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for the client service which ultimately calls to the external/defined service. 
    /// This is a contract interface
    /// </summary>
    public interface IShippingClient
    {
        /// <summary>
        /// Method which gets the shipping status
        /// </summary>
        /// <param name="request">The request object</param>
        /// <returns>The response containing the shipping data information</returns>
        Task<GetShippingStatusResponse> GetShippingStatusAsync(GetShippingStatusRequest request);
    }
}
