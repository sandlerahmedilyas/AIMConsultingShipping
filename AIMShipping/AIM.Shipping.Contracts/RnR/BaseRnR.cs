using System;
using System.Collections.Generic;
using System.Text;

namespace AIM.Shipping.Contracts.RnR
{
    public class BaseRequest
    {
        public BaseRequest()
        {

        }
    }

    public class BaseResponse
    {
        public bool Success { get; set; } = false;
        public string FailureInformation { get; set; } = string.Empty;


        public BaseResponse() { }
    }
}
