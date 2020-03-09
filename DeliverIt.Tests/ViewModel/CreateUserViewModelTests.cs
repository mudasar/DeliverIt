using DeliverIt.ViewModels.User;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Text;
using Xunit;
using Newtonsoft.Json;
using System.Net.Http;

namespace DeliverIt.Tests.ViewModel
{
    public class CreateUserViewModelTests
    {
        TestApplicationHelper helper;
        public CreateUserViewModelTests()
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
                var model = new CreateUserViewModel();
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                await client.PostAsync($"/api/user", content);
            });

        }
    }
}
