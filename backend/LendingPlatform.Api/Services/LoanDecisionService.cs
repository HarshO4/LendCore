using LendingPlatform.Api.Models.Results;
using LendingPlatform.Api.Policy;
using LendingPlatform.Api.Services.Interfaces;

namespace LendingPlatform.Api.Services;

/// <summary>
/// Implements the lending policy rules and produces a loan decision.
///
/// Rule evaluation order:
///   1. Minimum loan amount
///   2. Maximum loan amount
///   3. High-value loan (≥ £1,000,000)
///   4. Standard loan bands (LTV-based)
///
/// LTV is calculated as (LoanAmount / AssetValue) × 100 and stored as a
/// percentage value (e.g. 50 represents 50 %).
/// The unrounded value is used for all rule comparisons.
/// </summary>
public class LoanDecisionService : ILoanDecisionService
{
    public LoanDecisionResult Evaluate(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Calculate LTV as a percentage value — do NOT round before applying rules.
        decimal ltv = loanAmount / assetValue * 100m;

        // ── Rule 1: Minimum loan amount ──────────────────────────────────────
        if (loanAmount < LendingPolicy.MinimumLoanAmount)
        {
            return Declined(
                ltv,
                $"Loan amount is below the minimum permitted amount of £{LendingPolicy.MinimumLoanAmount:N0}.",
                "Minimum loan amount");
        }

        // ── Rule 2: Maximum loan amount ──────────────────────────────────────
        if (loanAmount > LendingPolicy.MaximumLoanAmount)
        {
            return Declined(
                ltv,
                $"Loan amount exceeds the maximum permitted amount of £{LendingPolicy.MaximumLoanAmount:N0}.",
                "Maximum loan amount");
        }

        // ── Rule 3: High-value loan (£1,000,000 and above) ──────────────────
        if (loanAmount >= LendingPolicy.HighValueLoanThreshold)
        {
            bool ltvOk    = ltv <= LendingPolicy.LtvHighValueMax;
            bool scoreOk  = creditScore >= LendingPolicy.CreditScoreThresholdHighValue;

            if (ltvOk && scoreOk)
            {
                return Approved(
                    ltv,
                    $"Loan approved. LTV is {ltv:F2}% which is at or below the high-value maximum of {LendingPolicy.LtvHighValueMax}% " +
                    $"and credit score of {creditScore} meets the required threshold of {LendingPolicy.CreditScoreThresholdHighValue}.",
                    "High-value loan rule");
            }

            return Declined(
                ltv,
                $"High-value loans of £{LendingPolicy.HighValueLoanThreshold:N0} or more require an LTV of " +
                $"{LendingPolicy.LtvHighValueMax}% or below and a credit score of at least " +
                $"{LendingPolicy.CreditScoreThresholdHighValue}. " +
                BuildHighValueFailureDetail(ltv, creditScore),
                "High-value loan rule");
        }

        // ── Rule 4: Standard loans (< £1,000,000) — LTV bands ───────────────

        // LTV ≥ 90 % → automatic decline regardless of credit score
        if (ltv >= LendingPolicy.LtvBand3Upper)
        {
            return Declined(
                ltv,
                $"LTV of {ltv:F2}% is at or above the maximum permitted 90%. No credit score can override this decline.",
                "LTV >= 90%");
        }

        // LTV in 80 %–<90 % band → requires credit score ≥ 900
        if (ltv >= LendingPolicy.LtvBand2Upper)
        {
            if (creditScore >= LendingPolicy.CreditScoreThresholdLtv80To90)
            {
                return Approved(
                    ltv,
                    $"Loan approved. LTV is {ltv:F2}% (80%–<90% band) and credit score of {creditScore} meets " +
                    $"the required threshold of {LendingPolicy.CreditScoreThresholdLtv80To90}.",
                    "Standard LTV 80%-<90%");
            }

            return Declined(
                ltv,
                $"Credit score of {creditScore} is below the required score of " +
                $"{LendingPolicy.CreditScoreThresholdLtv80To90} for the 80%–<90% LTV band.",
                "Standard LTV 80%-<90%");
        }

        // LTV in 60 %–<80 % band → requires credit score ≥ 800
        if (ltv >= LendingPolicy.LtvBand1Upper)
        {
            if (creditScore >= LendingPolicy.CreditScoreThresholdLtv60To80)
            {
                return Approved(
                    ltv,
                    $"Loan approved. LTV is {ltv:F2}% (60%–<80% band) and credit score of {creditScore} meets " +
                    $"the required threshold of {LendingPolicy.CreditScoreThresholdLtv60To80}.",
                    "Standard LTV 60%-<80%");
            }

            return Declined(
                ltv,
                $"Credit score of {creditScore} is below the required score of " +
                $"{LendingPolicy.CreditScoreThresholdLtv60To80} for the 60%–<80% LTV band.",
                "Standard LTV 60%-<80%");
        }

        // LTV < 60 % → requires credit score ≥ 750
        if (creditScore >= LendingPolicy.CreditScoreThresholdLtvBelow60)
        {
            return Approved(
                ltv,
                $"Loan approved. LTV is {ltv:F2}% (below 60% band) and credit score of {creditScore} meets " +
                $"the required threshold of {LendingPolicy.CreditScoreThresholdLtvBelow60}.",
                "Standard LTV < 60%");
        }

        return Declined(
            ltv,
            $"Credit score of {creditScore} is below the required score of " +
            $"{LendingPolicy.CreditScoreThresholdLtvBelow60} for the applicable LTV band (below 60%).",
            "Standard LTV < 60%");
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private static LoanDecisionResult Approved(decimal ltv, string reason, string rule) =>
        new() { IsApproved = true,  Ltv = ltv, DecisionReason = reason, RuleApplied = rule };

    private static LoanDecisionResult Declined(decimal ltv, string reason, string rule) =>
        new() { IsApproved = false, Ltv = ltv, DecisionReason = reason, RuleApplied = rule };

    private static string BuildHighValueFailureDetail(decimal ltv, int creditScore)
    {
        bool ltvFailed   = ltv > LendingPolicy.LtvHighValueMax;
        bool scoreFailed = creditScore < LendingPolicy.CreditScoreThresholdHighValue;

        if (ltvFailed && scoreFailed)
            return $"LTV of {ltv:F2}% exceeds the 60% limit and credit score of {creditScore} is below {LendingPolicy.CreditScoreThresholdHighValue}.";

        if (ltvFailed)
            return $"LTV of {ltv:F2}% exceeds the 60% limit.";

        return $"Credit score of {creditScore} is below the required {LendingPolicy.CreditScoreThresholdHighValue}.";
    }
}
