using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace CarInsurance.Api.Tests;

public sealed class InsuranceValidEndpointTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;
    public InsuranceValidEndpointTests(CustomWebAppFactory f)
        => _client = f.CreateClient(new() { AllowAutoRedirect = false });

    [Fact]
    public async Task EndDate_inclusive_returns_true()
    {
        var resp = await _client.GetAsync("api/cars/1/insurance-valid?date=2024-12-31");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("valid").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task NotFound_when_car_missing()
    {
        var resp = await _client.GetAsync("api/cars/9999/insurance-valid?date=2024-06-01");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task BadRequest_on_invalid_date()
    {
        var resp = await _client.GetAsync("api/cars/1/insurance-valid?date=bad");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
