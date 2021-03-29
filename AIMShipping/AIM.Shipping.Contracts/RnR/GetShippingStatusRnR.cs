using AIM.Shipping.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIM.Shipping.Contracts.RnR
{
    public sealed class GetShippingStatusRequest : BaseRequest
    {

    }

    public sealed class GetShippingStatusResponse : BaseResponse
    {
        public ShippingDataModel ShippingDataInformation { get; set; }

    }
}
