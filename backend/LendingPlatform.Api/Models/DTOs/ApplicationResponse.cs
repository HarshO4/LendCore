namespace LendingPlatform.Api.Models.DTOs;

/// <summary>
/// Response body returned after a loan application is evaluated and persisted.
/// </summary>
public class ApplicationResponse
{
    public int Id { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal AssetValue { get; set; }
    public int CreditScore { get; set; }

    /// <summary>
    /// LTV as a percentage value (e.g. 50.00 means 50 %).
    /// </summary>
    public decimal Ltv { get; set; }

    public string Decision { get; set; } = string.Empty;
    public string DecisionReason { get; set; } = string.Empty;
    public string RuleApplied { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
