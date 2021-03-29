namespace AIMShipping.Tests
{
    using AIM.Shipping.Contracts.RnR;
    using AIM.Shipping.Implementation;
    using AIM.Shipping.Models;
    using AIMShipping.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Newtonsoft.Json;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using Xunit;

    public class ShippingUnitTests
    {
        [Fact]
        public async void WhenHttpError_ShouldReturnNoSuccessResponseObject()
        {
            // Create our config, with some mocked uri

            var shippingServiceClientConfig = new ShippingServiceClientConfiguration { EndpointUrl = "http://someuri.com/api/service" };

            // create our expected response to test against
            var shipDate = DateTime.Now;
            var expectedData = new ShippingDataModel
            {
                ShipDate = shipDate,
                Complete = true,
                Status = "processing"
            };

            var expectedResponse = new GetShippingStatusResponse { Success = false };

            // mock our http client as we do not want to hit hit the actual end point
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

            var mockedHttpClient = httpClientFactoryMock.Object;

            // let's create our client class
            var service = new ShippingClient(new NullLogger<ShippingClient>(), mockedHttpClient, shippingServiceClientConfig);

            // let's get our result
            var testResult = await service.GetShippingStatusAsync(new GetShippingStatusRequest { });

            // Assert
            Assert.False(testResult.Success);
            Assert.True(testResult.IsExternalError);
        }

        [Fact]        
        public void WhenNoConfigSupplied_ExpectArgumentNullException()
        {
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });

            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

            var mockedHttpClient = httpClientFactoryMock.Object;

            // Assert
            Assert.Throws<ArgumentNullException>(() => new ShippingClient(new NullLogger<ShippingClient>(), mockedHttpClient, null));
        }

        [Fact]
        public void WhenNoClientFactorySupplied_ExpectArgumentNullException()
        {
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });

            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

            // Assert
            Assert.Throws<ArgumentNullException>(() => new ShippingClient(new NullLogger<ShippingClient>(), null, null));
        }

        [Fact]
        public async void WhenUrlSupplied_ShouldReturnShipData()
        {
            // Create our config, with some mocked uri

            var shippingServiceClientConfig = new ShippingServiceClientConfiguration { EndpointUrl = "http://someuri.com/api/service" };
            
            // create our expected response to test against
            var shipDate = DateTime.Now;
            var expectedData = new ShippingDataModel
            {
                ShipDate = shipDate,
                Complete = true,
                Status = "processing"
            };

            var expectedResponse = new GetShippingStatusResponse { Success = true, ShippingDataInformation = expectedData };

            // mock our http client as we do not want to hit hit the actual end point
            var httpClientFactoryMock = new Mock<IHttpClientFactory>(); 
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // json serializer settings for setting camel case options
            var ser = new JsonSerializerSettings();

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(expectedData, serializerOptions))
            });

            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

            var mockedHttpClient = httpClientFactoryMock.Object;

            // let's create our client class
            var service = new ShippingClient(new NullLogger<ShippingClient>(), mockedHttpClient, shippingServiceClientConfig);

            // let's get our result
            var testResult = await service.GetShippingStatusAsync(new GetShippingStatusRequest { });

            // Assert
            Assert.True(testResult.Success == true);
            Assert.True(testResult.ShippingDataInformation.ShipDate == expectedResponse.ShippingDataInformation.ShipDate);
        }
    }
}
