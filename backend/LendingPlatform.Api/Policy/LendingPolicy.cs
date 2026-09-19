namespace LendingPlatform.Api.Policy;

/// <summary>
/// Centralised lending policy constants.
/// All business-rule thresholds live here so that no magic numbers
/// are scattered across service or test code.
/// </summary>
public static class LendingPolicy
{
    // ── Loan amount limits ──────────────────────────────────────────────
    public const decimal MinimumLoanAmount = 100_000m;
    public const decimal MaximumLoanAmount = 1_500_000m;
    public const decimal HighValueLoanThreshold = 1_000_000m;

    // ── LTV bands (as percentage values, e.g. 60 = 60 %) ──────────────
    public const decimal LtvBand1Upper = 60m;   // < 60
    public const decimal LtvBand2Upper = 80m;   // 60 ≤ x < 80
    public const decimal LtvBand3Upper = 90m;   // 80 ≤ x < 90
    // ≥ 90 → automatic decline

    // ── Credit-score requirements ───────────────────────────────────────
    public const int MinCreditScore = 1;
    public const int MaxCreditScore = 999;

    public const int CreditScoreThresholdLtvBelow60  = 750;
    public const int CreditScoreThresholdLtv60To80   = 800;
    public const int CreditScoreThresholdLtv80To90   = 900;
    public const int CreditScoreThresholdHighValue    = 950;
    public const decimal LtvHighValueMax              = 60m;
}
