namespace LendingPlatform.Api.Models.Entities;

/// <summary>
/// Persisted loan application record.
/// </summary>
public class LoanApplication
{
    public int Id { get; set; }

    /// <summary>Requested loan amount in GBP.</summary>
    public decimal LoanAmount { get; set; }

    /// <summary>Value of the asset used as security in GBP.</summary>
    public decimal AssetValue { get; set; }

    /// <summary>Applicant credit score (1–999).</summary>
    public int CreditScore { get; set; }

    /// <summary>
    /// Loan-to-Value ratio stored as a percentage value.
    /// E.g. 50 means 50 %, not 0.50.
    /// </summary>
    public decimal Ltv { get; set; }

    /// <summary>"Approved" or "Declined".</summary>
    public string Decision { get; set; } = string.Empty;

    /// <summary>Human-readable explanation of the decision.</summary>
    public string DecisionReason { get; set; } = string.Empty;

    /// <summary>Concise label for the rule that determined the outcome.</summary>
    public string RuleApplied { get; set; } = string.Empty;

    /// <summary>UTC timestamp when the application was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
