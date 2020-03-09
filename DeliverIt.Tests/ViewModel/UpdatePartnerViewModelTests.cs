using DeliverIt.ViewModels.Partner;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using Xunit;

namespace DeliverIt.Tests.ViewModel
{
    public class UpdatePartnerViewModelTests
    {
        TestApplicationHelper helper;
        public UpdatePartnerViewModelTests()
        {
            helper = new TestApplicationHelper();

        }

        [Fact]
        public void ReturnsValidationErrorIfMissingRequiredFields()
        {
            Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                var host = helper.GetHost().Build();
                await host.StartAsync();
                var client = host.GetTestClient();
                var model = new UpdatePartnerViewModel();
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                await client.PutAsync($"/api/partner", content);
            });

        }
    }
}
