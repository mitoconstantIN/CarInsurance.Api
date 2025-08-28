using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace CarInsurance.Api.Tests;

public sealed class ClaimsEndpointSmokeTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;
    public ClaimsEndpointSmokeTests(CustomWebAppFactory f) => _client = f.CreateClient();

    [Fact]
    public async Task CreateClaim_returns_201_and_Location()
    {
        var b = new { claimDate = "2025-08-26", description = "test", amount = 100m };
        var r = await _client.PostAsJsonAsync("api/cars/1/claims", b);
        r.StatusCode.Should().Be(HttpStatusCode.Created);
        r.Headers.Location!.ToString().Should().StartWith("/api/cars/1/claims/");
    }

    [Fact]
    public async Task CreateClaim_400_on_invalid_amount()
    {
        var b = new { claimDate = "2025-08-26", description = "x", amount = 0m };
        var r = await _client.PostAsJsonAsync("api/cars/1/claims", b);
        r.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
