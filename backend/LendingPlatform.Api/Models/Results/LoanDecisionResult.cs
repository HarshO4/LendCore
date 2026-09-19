namespace LendingPlatform.Api.Models.Results;

/// <summary>
/// The outcome of evaluating a loan application against the lending policy.
/// </summary>
public class LoanDecisionResult
{
    public bool IsApproved { get; init; }
    public decimal Ltv { get; init; }
    public string Decision => IsApproved ? "Approved" : "Declined";
    public string DecisionReason { get; init; } = string.Empty;
    public string RuleApplied { get; init; } = string.Empty;
}
