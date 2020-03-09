using DeliverIt.ViewModels.User;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using Xunit;

namespace DeliverIt.Tests.ViewModel
{
    public class UpdateUserViewModelTests
    {
        TestApplicationHelper helper;
        public UpdateUserViewModelTests()
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
                var model = new UpdateUserViewModel();
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                await client.PutAsync($"/api/user", content);
            });

        }
    }
}
