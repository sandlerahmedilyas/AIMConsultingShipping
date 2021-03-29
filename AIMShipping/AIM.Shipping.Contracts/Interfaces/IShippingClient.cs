using AIM.Shipping.Contracts.RnR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIM.Shipping.Contracts.Interfaces
{
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
