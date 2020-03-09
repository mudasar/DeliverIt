using DeliverIt.ViewModels.Partner;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace DeliverIt.Tests.ViewModel
{
    public class CreatePartnerViewModelTests
    {
        TestApplicationHelper helper;
        public CreatePartnerViewModelTests()
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
                var model = new CreatePartnerViewModel();
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                await client.PostAsync($"/api/partner", content);
            });

        }
    }
}
