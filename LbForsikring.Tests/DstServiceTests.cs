using System.Net;
using LbForsikring.Integrations;
using Microsoft.Extensions.DependencyInjection;

namespace LbForsikring.Tests;

public class DstServiceTests
{
    [Fact]
    public async Task GetIndustryStats_WhenServiceTemporarilyUnavailable_RetriesAndSucceeds()
    {
        // Arrange
        var requestCount = 0;

        var handler = new FakeHttpMessageHandler(_ =>
        {
            requestCount++;

            if (requestCount < 3)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """
                    BRANCHE07;TAL;TID;INDHOLD
                    471120 Supermarkeder;Arbejdssteder ultimo november;2024;854
                    """)
            };
        });

        var services = new ServiceCollection();

        services.AddHttpClient<IDstService, DstService>()
            .ConfigurePrimaryHttpMessageHandler(() => handler)
            .AddStandardResilienceHandler();

        var provider = services.BuildServiceProvider();

        var service = provider.GetRequiredService<IDstService>();

        // Act
        var result = await service.GetIndustryStats("471120", 2024);

        // Assert
        Assert.Contains("471120", result);
        Assert.Equal(3, requestCount);
    }

    [Fact]
    public async Task GetIndustryStats_WhenBadRequest_DoesNotRetry()
    {
        // Arrange
        var requestCount = 0;

        var handler = new FakeHttpMessageHandler(_ =>
        {
            requestCount++;

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        });

        var services = new ServiceCollection();

        services.AddHttpClient<IDstService, DstService>()
            .ConfigurePrimaryHttpMessageHandler(() => handler)
            .AddStandardResilienceHandler();

        var provider = services.BuildServiceProvider();

        var service = provider.GetRequiredService<IDstService>();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            service.GetIndustryStats("471120", 2024));

        Assert.Equal(1, requestCount);
    }
}

public class FakeHttpMessageHandler(
    Func<HttpRequestMessage, HttpResponseMessage> handler)
    : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(handler(request));
    }
}