using LendingPlatform.Api.Data;
using LendingPlatform.Api.Models.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LendingPlatform.Tests.Api;

/// <summary>
/// Integration tests for the /api/applications and /api/statistics endpoints.
/// Uses an in-memory database and the ASP.NET Core test host.
/// </summary>
public class ApplicationsApiTests : IClassFixture<LendingApiWebApplicationFactory>
{
    private readonly LendingApiWebApplicationFactory _factory;

    public ApplicationsApiTests(LendingApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient() => _factory.CreateClient();

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/applications — VALID APPROVED
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Post_ValidApprovedApplication_Returns201_WithDecision()
    {
        var client = CreateClient();
        var request = new CreateApplicationRequest
        {
            LoanAmount  = 500_000m,
            AssetValue  = 1_000_000m,
            CreditScore = 800
        };

        var response = await client.PostAsJsonAsync("/api/applications", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApplicationResponse>();
        Assert.NotNull(body);
        Assert.Equal("Approved", body!.Decision);
        Assert.Equal(50m, body.Ltv);
        Assert.True(body.Id > 0);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/applications — VALID DECLINED (below minimum)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Post_ValidDeclinedApplication_Returns201_WithDeclinedDecision()
    {
        var client = CreateClient();
        var request = new CreateApplicationRequest
        {
            LoanAmount  = 50_000m,
            AssetValue  = 100_000m,
            CreditScore = 900
        };

        var response = await client.PostAsJsonAsync("/api/applications", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApplicationResponse>();
        Assert.NotNull(body);
        Assert.Equal("Declined", body!.Decision);
        Assert.True(body.Id > 0); // was persisted
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/applications — INVALID inputs → 400
    // ─────────────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0,       1_000_000, 750)]  // loan = 0
    [InlineData(-1,      1_000_000, 750)]  // negative loan
    [InlineData(500_000, 0,         750)]  // asset = 0
    [InlineData(500_000, -1,        750)]  // negative asset
    [InlineData(500_000, 1_000_000, 0)]    // credit score = 0
    [InlineData(500_000, 1_000_000, 1000)] // credit score = 1000
    [InlineData(500_000, 1_000_000, -1)]   // negative credit score
    public async Task Post_InvalidInputs_Returns400(
        decimal loanAmount, decimal assetValue, int creditScore)
    {
        var client = CreateClient();
        var request = new { LoanAmount = loanAmount, AssetValue = assetValue, CreditScore = creditScore };

        var response = await client.PostAsJsonAsync("/api/applications", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GET /api/applications
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Get_Applications_ReturnsOk()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/api/applications");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<ApplicationResponse>>();
        Assert.NotNull(body);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GET /api/statistics
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Get_Statistics_ReturnsOk()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/api/statistics");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<StatisticsResponse>();
        Assert.NotNull(body);
    }

    [Fact]
    public async Task Get_Statistics_AfterApprovedApplication_CountsCorrectly()
    {
        // Use a fresh factory for isolated state
        await using var isolatedFactory = new LendingApiWebApplicationFactory();
        var client = isolatedFactory.CreateClient();

        await client.PostAsJsonAsync("/api/applications", new CreateApplicationRequest
        {
            LoanAmount = 500_000m, AssetValue = 1_000_000m, CreditScore = 800
        });

        var response = await client.GetAsync("/api/statistics");
        var stats = await response.Content.ReadFromJsonAsync<StatisticsResponse>();

        Assert.NotNull(stats);
        Assert.Equal(1, stats!.TotalApplicants);
        Assert.Equal(1, stats.ApprovedCount);
        Assert.Equal(0, stats.DeclinedCount);
        Assert.Equal(500_000m, stats.TotalValueOfApprovedLoans);
        Assert.NotNull(stats.AverageLtv);
    }

    [Fact]
    public async Task Post_InvalidApplication_DoesNotIncrementStatistics()
    {
        await using var isolatedFactory = new LendingApiWebApplicationFactory();
        var client = isolatedFactory.CreateClient();

        // Submit an invalid application (loan = 0)
        await client.PostAsJsonAsync("/api/applications", new { LoanAmount = 0, AssetValue = 1_000_000, CreditScore = 800 });

        var response = await client.GetAsync("/api/statistics");
        var stats = await response.Content.ReadFromJsonAsync<StatisticsResponse>();

        Assert.NotNull(stats);
        Assert.Equal(0, stats!.TotalApplicants);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GET /api/health
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Get_Health_ReturnsOk()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/api/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
