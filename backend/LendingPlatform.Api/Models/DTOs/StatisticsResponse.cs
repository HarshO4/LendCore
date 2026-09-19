namespace LendingPlatform.Api.Models.DTOs;

/// <summary>
/// Portfolio statistics derived from all persisted applications.
/// </summary>
public class StatisticsResponse
{
    /// <summary>Total number of valid persisted applications.</summary>
    public int TotalApplicants { get; set; }

    /// <summary>Number of approved applications.</summary>
    public int ApprovedCount { get; set; }

    /// <summary>Number of declined applications.</summary>
    public int DeclinedCount { get; set; }

    /// <summary>Sum of loan amounts for approved applications only.</summary>
    public decimal TotalValueOfApprovedLoans { get; set; }

    /// <summary>
    /// Average LTV across ALL valid persisted applications (approved and declined).
    /// Null when the database is empty.
    /// </summary>
    public decimal? AverageLtv { get; set; }
}
