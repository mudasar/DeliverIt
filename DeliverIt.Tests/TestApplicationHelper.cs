using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverIt.Tests
{
    public class TestApplicationHelper
    {

        public IHostBuilder GetHost()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    // Add TestServer
                    webHost.UseTestServer();

                    // Specify the environment
                    webHost.UseEnvironment("Test");

                    webHost.Configure(app => app.Run(async ctx => await ctx.Response.WriteAsync("Hello World!")));
                });
            return hostBuilder;
        }
    }
}
