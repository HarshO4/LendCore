using LendingPlatform.Api.Data;
using LendingPlatform.Api.Models.Entities;
using LendingPlatform.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LendingPlatform.Tests.Services;

/// <summary>
/// Unit tests for <see cref="StatisticsService"/> using an in-memory database.
/// </summary>
public class StatisticsServiceTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly StatisticsService _sut;

    public StatisticsServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // isolated per test
            .Options;

        _db  = new ApplicationDbContext(options);
        _sut = new StatisticsService(_db);
    }

    public void Dispose() => _db.Dispose();

    // ─────────────────────────────────────────────────────────────────────────
    // EMPTY DATABASE
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task EmptyDatabase_Returns_ZeroTotals_AndNullAverageLtv()
    {
        var stats = await _sut.GetStatisticsAsync();

        Assert.Equal(0, stats.TotalApplicants);
        Assert.Equal(0, stats.ApprovedCount);
        Assert.Equal(0, stats.DeclinedCount);
        Assert.Equal(0m, stats.TotalValueOfApprovedLoans);
        Assert.Null(stats.AverageLtv);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // APPROVED ONLY
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task ApprovedOnly_CountsCorrectly()
    {
        SeedApplication(decision: "Approved", loanAmount: 500_000m, ltv: 50m);
        SeedApplication(decision: "Approved", loanAmount: 300_000m, ltv: 40m);
        await _db.SaveChangesAsync();

        var stats = await _sut.GetStatisticsAsync();

        Assert.Equal(2, stats.TotalApplicants);
        Assert.Equal(2, stats.ApprovedCount);
        Assert.Equal(0, stats.DeclinedCount);
        Assert.Equal(800_000m, stats.TotalValueOfApprovedLoans);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // DECLINED ONLY
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeclinedOnly_CountsCorrectly_ApprovedValueIsZero()
    {
        SeedApplication(decision: "Declined", loanAmount: 200_000m, ltv: 95m);
        await _db.SaveChangesAsync();

        var stats = await _sut.GetStatisticsAsync();

        Assert.Equal(1, stats.TotalApplicants);
        Assert.Equal(0, stats.ApprovedCount);
        Assert.Equal(1, stats.DeclinedCount);
        Assert.Equal(0m, stats.TotalValueOfApprovedLoans);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // MIXED — approved + declined
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Mixed_TotalsAreCorrect()
    {
        SeedApplication(decision: "Approved", loanAmount: 500_000m, ltv: 50m);
        SeedApplication(decision: "Approved", loanAmount: 700_000m, ltv: 55m);
        SeedApplication(decision: "Declined", loanAmount: 200_000m, ltv: 95m);
        SeedApplication(decision: "Declined", loanAmount: 150_000m, ltv: 90m);
        await _db.SaveChangesAsync();

        var stats = await _sut.GetStatisticsAsync();

        Assert.Equal(4, stats.TotalApplicants);
        Assert.Equal(2, stats.ApprovedCount);
        Assert.Equal(2, stats.DeclinedCount);
        Assert.Equal(1_200_000m, stats.TotalValueOfApprovedLoans);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AVERAGE LTV
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task AverageLtv_IncludesApprovedAndDeclined()
    {
        SeedApplication(decision: "Approved", loanAmount: 500_000m, ltv: 50m);
        SeedApplication(decision: "Declined", loanAmount: 200_000m, ltv: 90m);
        await _db.SaveChangesAsync();

        var stats = await _sut.GetStatisticsAsync();

        // (50 + 90) / 2 = 70
        Assert.NotNull(stats.AverageLtv);
        Assert.Equal(70m, stats.AverageLtv!.Value);
    }

    [Fact]
    public async Task AverageLtv_DoesNotOnlyAverageApproved()
    {
        SeedApplication(decision: "Approved", loanAmount: 500_000m, ltv: 50m);
        SeedApplication(decision: "Declined", loanAmount: 200_000m, ltv: 90m);
        await _db.SaveChangesAsync();

        var stats = await _sut.GetStatisticsAsync();

        // If it only averaged approved it would be 50; correct value is 70
        Assert.NotEqual(50m, stats.AverageLtv);
    }

    [Fact]
    public async Task TotalValueOfApprovedLoans_OnlyIncludesApproved()
    {
        SeedApplication(decision: "Approved", loanAmount: 500_000m, ltv: 50m);
        SeedApplication(decision: "Declined", loanAmount: 400_000m, ltv: 95m);
        await _db.SaveChangesAsync();

        var stats = await _sut.GetStatisticsAsync();

        Assert.Equal(500_000m, stats.TotalValueOfApprovedLoans);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────────────────────────────────

    private void SeedApplication(string decision, decimal loanAmount, decimal ltv)
    {
        _db.LoanApplications.Add(new LoanApplication
        {
            LoanAmount     = loanAmount,
            AssetValue     = 1_000_000m,
            CreditScore    = 800,
            Ltv            = ltv,
            Decision       = decision,
            DecisionReason = "Test reason",
            RuleApplied    = "Test rule",
            CreatedAt      = DateTime.UtcNow
        });
    }
}
