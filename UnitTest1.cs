using Prosto;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;

namespace TestProsto;

public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient httpClient;

    public UnitTest1()
    {
        var factory = new WebApplicationFactory<Program>();
        _factory = factory;
        httpClient = factory.CreateClient();
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Home/Category")]
    [InlineData("/UserProfile")]
    [InlineData("/Item")]
    [InlineData("/Home/Contacts")]
    public async void PagesLoad(string path)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync(path);
        int code = (int)response.StatusCode;
        Assert.Equal(200, code);
    }

    [Theory]
    [InlineData("17")]
    [InlineData("Iphone")]
    [InlineData("pro")]
    [InlineData("256")]
    [InlineData("Deep")]
    public async void PageContent(string content)
    {
        var response = await httpClient.GetAsync("/Home/Category");
        var pageContent = await response.Content.ReadAsStringAsync();
        string contentString = pageContent.ToString();
        Assert.Contains(content, contentString);
    }
}