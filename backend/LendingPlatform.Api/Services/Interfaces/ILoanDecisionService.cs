using LendingPlatform.Api.Models.Results;

namespace LendingPlatform.Api.Services.Interfaces;

/// <summary>
/// Evaluates a loan application against the lending policy and returns a decision.
/// The service is pure (no I/O) so it can be tested without any infrastructure.
/// </summary>
public interface ILoanDecisionService
{
    /// <summary>
    /// Evaluate the supplied inputs and return the lending decision.
    /// </summary>
    /// <param name="loanAmount">Requested loan amount in GBP.</param>
    /// <param name="assetValue">Security asset value in GBP.</param>
    /// <param name="creditScore">Applicant credit score (1–999).</param>
    /// <returns>
    /// A <see cref="LoanDecisionResult"/> containing the LTV, the decision,
    /// a human-readable reason, and the rule label.
    /// </returns>
    LoanDecisionResult Evaluate(decimal loanAmount, decimal assetValue, int creditScore);
}
