namespace AIM.Shipping.Contracts.RnR
{
    using AIM.Shipping.Models;

    public sealed class GetShippingStatusRequest : BaseRequest
    {

    }

    public sealed class GetShippingStatusResponse : BaseResponse
    {
        public ShippingDataModel ShippingDataInformation { get; set; }

        public bool IsExternalError { get; set; }
    }
}
