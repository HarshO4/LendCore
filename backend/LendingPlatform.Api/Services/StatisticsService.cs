using LendingPlatform.Api.Data;
using LendingPlatform.Api.Models.DTOs;
using LendingPlatform.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LendingPlatform.Api.Services;

/// <summary>
/// Computes portfolio statistics from all persisted applications.
/// Average LTV covers both approved and declined applications.
/// </summary>
public class StatisticsService : IStatisticsService
{
    private readonly ApplicationDbContext _db;

    public StatisticsService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<StatisticsResponse> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var applications = await _db.LoanApplications
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        int total    = applications.Count;
        int approved = applications.Count(a => a.Decision == "Approved");
        int declined = applications.Count(a => a.Decision == "Declined");

        decimal totalApprovedValue = applications
            .Where(a => a.Decision == "Approved")
            .Sum(a => a.LoanAmount);

        decimal? averageLtv = total > 0
            ? applications.Average(a => a.Ltv)
            : null;

        return new StatisticsResponse
        {
            TotalApplicants            = total,
            ApprovedCount              = approved,
            DeclinedCount              = declined,
            TotalValueOfApprovedLoans  = totalApprovedValue,
            AverageLtv                 = averageLtv
        };
    }
}
