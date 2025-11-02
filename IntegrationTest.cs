using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prosto;
using Prosto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;


namespace TestProsto
{
    public class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public IntegrationTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                // Читаємо appsettings.json — той самий, що в додатку
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                });

                builder.ConfigureServices(services =>
                {
                    // Вимкнути авторизацію — щоб тести могли заходити
                    services.AddAuthorization(options => options.FallbackPolicy = null);
                });
            });
        }

        [Fact]
        public async Task Home_Index_Loads()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Home/Index");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Home_Category_Loads()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Home/Category?category=Електроніка");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Item_Index_ReturnsNotFound_ForInvalidId()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Item/Index/999999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UserProfile_RedirectsToLogin_WhenNotAuthenticated()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/UserProfile/Index");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/UserProfile/Login", response.Headers.Location?.OriginalString);
        }

        [Fact]
        public async Task Order_RedirectsToLogin_WhenNotAuthenticated()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/Order/Index");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/UserProfile/Login", response.Headers.Location?.OriginalString);
        }
    }
}
