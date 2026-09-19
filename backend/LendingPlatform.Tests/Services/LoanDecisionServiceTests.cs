using LendingPlatform.Api.Services;
using Xunit;

namespace LendingPlatform.Tests.Services;

/// <summary>
/// Unit tests for <see cref="LoanDecisionService"/>.
/// Covers all business rules, boundaries, and edge cases.
/// </summary>
public class LoanDecisionServiceTests
{
    private readonly LoanDecisionService _sut = new();

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Build a loan request with a known LTV using simple round numbers.
    /// ltv is the desired percentage, e.g. pass 50 for 50 %.
    /// </summary>
    private static (decimal loan, decimal asset) ForLtv(decimal ltv, decimal loanAmount = 500_000m)
    {
        // asset = loan / (ltv / 100)
        decimal asset = loanAmount / (ltv / 100m);
        return (loanAmount, asset);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // INPUT VALIDATION — boundary values
    // (Note: validation is enforced at the DTO layer / ModelState; the service
    //  receives already-validated values, so these tests confirm the service
    //  itself does NOT error on extreme valid inputs.)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void CreditScore_1_IsAccepted_AsValidInput()
    {
        // Minimum valid credit score should not throw
        var result = _sut.Evaluate(200_000m, 1_000_000m, 1);
        Assert.NotNull(result);
    }

    [Fact]
    public void CreditScore_999_IsAccepted_AsValidInput()
    {
        var result = _sut.Evaluate(200_000m, 1_000_000m, 999);
        Assert.NotNull(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RULE 1 — Minimum loan amount
    // ─────────────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(99_999)]
    [InlineData(50_000)]
    [InlineData(1)]
    public void LoanBelow100k_IsDeclined(decimal loanAmount)
    {
        var result = _sut.Evaluate(loanAmount, 1_000_000m, 999);

        Assert.False(result.IsApproved);
        Assert.Equal("Minimum loan amount", result.RuleApplied);
    }

    [Fact]
    public void Loan_Exactly99999_IsDeclined_WithMinimumAmountRule()
    {
        var result = _sut.Evaluate(99_999m, 1_000_000m, 999);

        Assert.False(result.IsApproved);
        Assert.Equal("Minimum loan amount", result.RuleApplied);
    }

    [Fact]
    public void Loan_Exactly100000_IsEvaluatedNormally_NotDeclinedByMinimumRule()
    {
        // £100,000 with a great LTV and credit score should be approved.
        var result = _sut.Evaluate(100_000m, 1_000_000m, 999);

        Assert.True(result.IsApproved);
        Assert.NotEqual("Minimum loan amount", result.RuleApplied);
    }

    [Fact]
    public void Loan_100001_IsEvaluatedNormally()
    {
        var result = _sut.Evaluate(100_001m, 1_000_000m, 999);

        Assert.True(result.IsApproved);
        Assert.NotEqual("Minimum loan amount", result.RuleApplied);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RULE 2 — Maximum loan amount
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void Loan_1500001_IsDeclined_WithMaximumAmountRule()
    {
        var result = _sut.Evaluate(1_500_001m, 5_000_000m, 999);

        Assert.False(result.IsApproved);
        Assert.Equal("Maximum loan amount", result.RuleApplied);
    }

    [Theory]
    [InlineData(2_000_000)]
    [InlineData(5_000_000)]
    public void LoanAbove1500000_IsDeclined_WithMaximumAmountRule(decimal loanAmount)
    {
        var result = _sut.Evaluate(loanAmount, 20_000_000m, 999);

        Assert.False(result.IsApproved);
        Assert.Equal("Maximum loan amount", result.RuleApplied);
    }

    [Fact]
    public void Loan_Exactly1500000_IsEvaluatedNormally_NotDeclinedByMaximumRule()
    {
        // £1,500,000 with LTV <= 60% and score >= 950 should be approved (high-value rule).
        // 1,500,000 / 3,000,000 * 100 = 50 % LTV
        var result = _sut.Evaluate(1_500_000m, 3_000_000m, 950);

        Assert.True(result.IsApproved);
        Assert.Equal("High-value loan rule", result.RuleApplied);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RULE 3 — High-value loan (≥ £1,000,000)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void HighValue_LTV50_Score950_IsApproved()
    {
        // 1,000,000 / 2,000,000 * 100 = 50 %
        var result = _sut.Evaluate(1_000_000m, 2_000_000m, 950);

        Assert.True(result.IsApproved);
        Assert.Equal("High-value loan rule", result.RuleApplied);
    }

    [Fact]
    public void HighValue_LtvExactly60_Score950_IsApproved()
    {
        // LTV = 60 % exactly for a high-value loan — at the boundary, should still be approved.
        // 1,200,000 / 2,000,000 * 100 = 60 %
        var result = _sut.Evaluate(1_200_000m, 2_000_000m, 950);

        Assert.True(result.IsApproved);
        Assert.Equal("High-value loan rule", result.RuleApplied);
    }

    [Fact]
    public void HighValue_LtvExactly60_Score949_IsDeclined()
    {
        // LTV = 60 % exactly for a high-value loan — score fails the 950 threshold.
        // 1,200,000 / 2,000,000 * 100 = 60 %
        var result = _sut.Evaluate(1_200_000m, 2_000_000m, 949);

        Assert.False(result.IsApproved);
        Assert.Equal("High-value loan rule", result.RuleApplied);
    }

    [Fact]
    public void HighValue_LtvAbove60_IsDeclined()
    {
        // LTV = 75 % — above the 60 % cap
        var result = _sut.Evaluate(1_500_000m, 2_000_000m, 999);

        Assert.False(result.IsApproved);
        Assert.Equal("High-value loan rule", result.RuleApplied);
    }

    [Fact]
    public void HighValue_LTV50_Score949_IsDeclined()
    {
        var result = _sut.Evaluate(1_000_000m, 2_000_000m, 949);

        Assert.False(result.IsApproved);
        Assert.Equal("High-value loan rule", result.RuleApplied);
    }

    [Fact]
    public void HighValue_ExactlyOneMillion_IsSubjectToHighValueRule()
    {
        // Exactly £1,000,000 must use the high-value rule, not standard bands.
        var result = _sut.Evaluate(1_000_000m, 2_000_000m, 800);

        Assert.False(result.IsApproved);
        Assert.Equal("High-value loan rule", result.RuleApplied);
    }

    [Fact]
    public void StandardLoan_999999_IsNotSubjectToHighValueRule()
    {
        // £999,999 just below the threshold — should use standard LTV bands.
        // LTV = 999999 / 2000000 * 100 = 49.99..% — below 60%, needs score >= 750
        var result = _sut.Evaluate(999_999m, 2_000_000m, 750);

        Assert.True(result.IsApproved);
        Assert.Equal("Standard LTV < 60%", result.RuleApplied);
    }

    [Fact]
    public void HighValue_1000001_IsSubjectToHighValueRule()
    {
        var result = _sut.Evaluate(1_000_001m, 2_000_000m, 950);

        Assert.True(result.IsApproved);
        Assert.Equal("High-value loan rule", result.RuleApplied);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RULE 4 — Standard loans, LTV < 60 %
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void Standard_LtvBelow60_Score750_IsApproved()
    {
        // 500,000 / 1,100,000 * 100 ≈ 45.45 %
        var result = _sut.Evaluate(500_000m, 1_100_000m, 750);

        Assert.True(result.IsApproved);
        Assert.Equal("Standard LTV < 60%", result.RuleApplied);
    }

    [Fact]
    public void Standard_LtvBelow60_Score749_IsDeclined()
    {
        var result = _sut.Evaluate(500_000m, 1_100_000m, 749);

        Assert.False(result.IsApproved);
        Assert.Equal("Standard LTV < 60%", result.RuleApplied);
    }

    [Fact]
    public void Standard_LtvBelow60_Score999_IsApproved()
    {
        var result = _sut.Evaluate(200_000m, 1_000_000m, 999);

        Assert.True(result.IsApproved);
        Assert.Equal("Standard LTV < 60%", result.RuleApplied);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RULE 4 — Standard loans, LTV 60 %–<80 %
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void Standard_LtvExactly60_Score800_IsApproved()
    {
        // LTV = 60 % exactly belongs to the 60–<80 band
        var (loan, asset) = ForLtv(60m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 800);

        Assert.True(result.IsApproved);
        Assert.Equal("Standard LTV 60%-<80%", result.RuleApplied);
    }

    [Fact]
    public void Standard_LtvExactly60_Score799_IsDeclined()
    {
        var (loan, asset) = ForLtv(60m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 799);

        Assert.False(result.IsApproved);
        Assert.Equal("Standard LTV 60%-<80%", result.RuleApplied);
    }

    [Fact]
    public void Standard_Ltv70_Score800_IsApproved()
    {
        var (loan, asset) = ForLtv(70m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 800);

        Assert.True(result.IsApproved);
        Assert.Equal("Standard LTV 60%-<80%", result.RuleApplied);
    }

    [Fact]
    public void Standard_Ltv79_Score800_IsApproved()
    {
        var (loan, asset) = ForLtv(79m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 800);

        Assert.True(result.IsApproved);
        Assert.Equal("Standard LTV 60%-<80%", result.RuleApplied);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RULE 4 — Standard loans, LTV 80 %–<90 %
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void Standard_LtvExactly80_Score900_IsApproved()
    {
        // LTV = 80 % exactly belongs to the 80–<90 band
        var (loan, asset) = ForLtv(80m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 900);

        Assert.True(result.IsApproved);
        Assert.Equal("Standard LTV 80%-<90%", result.RuleApplied);
    }

    [Fact]
    public void Standard_LtvExactly80_Score899_IsDeclined()
    {
        var (loan, asset) = ForLtv(80m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 899);

        Assert.False(result.IsApproved);
        Assert.Equal("Standard LTV 80%-<90%", result.RuleApplied);
    }

    [Fact]
    public void Standard_Ltv85_Score900_IsApproved()
    {
        var (loan, asset) = ForLtv(85m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 900);

        Assert.True(result.IsApproved);
        Assert.Equal("Standard LTV 80%-<90%", result.RuleApplied);
    }

    [Fact]
    public void Standard_Ltv89_Score900_IsApproved()
    {
        // Just below 90 % — should use 80-<90 band
        var (loan, asset) = ForLtv(89m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 900);

        Assert.True(result.IsApproved);
        Assert.Equal("Standard LTV 80%-<90%", result.RuleApplied);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RULE 4 — Standard loans, LTV ≥ 90 % → automatic decline
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void Standard_LtvExactly90_IsDeclined_RegardlessOfScore()
    {
        var (loan, asset) = ForLtv(90m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 999);

        Assert.False(result.IsApproved);
        Assert.Equal("LTV >= 90%", result.RuleApplied);
    }

    [Fact]
    public void Standard_Ltv95_IsDeclined_RegardlessOfScore()
    {
        var (loan, asset) = ForLtv(95m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 999);

        Assert.False(result.IsApproved);
        Assert.Equal("LTV >= 90%", result.RuleApplied);
    }

    [Fact]
    public void Standard_Ltv100_IsDeclined_RegardlessOfScore()
    {
        var result = _sut.Evaluate(500_000m, 500_000m, 999);

        Assert.False(result.IsApproved);
        Assert.Equal("LTV >= 90%", result.RuleApplied);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // LTV CALCULATION
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void LtvCalculation_Loan500k_Asset1M_Is50Percent()
    {
        var result = _sut.Evaluate(500_000m, 1_000_000m, 800);

        Assert.Equal(50m, result.Ltv);
    }

    [Fact]
    public void LtvCalculation_Loan500k_Asset625k_Is80Percent()
    {
        // Test case 2 from spec: 500,000 / 625,000 * 100 = 80 %
        var result = _sut.Evaluate(500_000m, 625_000m, 800);

        Assert.Equal(80m, result.Ltv);
        Assert.False(result.IsApproved); // score 800 fails the 80-<90 band (needs 900)
    }

    [Fact]
    public void LtvCalculation_Loan1M_Asset2M_Is50Percent()
    {
        var result = _sut.Evaluate(1_000_000m, 2_000_000m, 950);

        Assert.Equal(50m, result.Ltv);
        Assert.True(result.IsApproved);
    }

    [Fact]
    public void LtvCalculation_Loan1500k_Asset2M_Is75Percent()
    {
        // Test case 6: 1,500,000 / 2,000,000 * 100 = 75 %
        var result = _sut.Evaluate(1_500_000m, 2_000_000m, 999);

        Assert.Equal(75m, result.Ltv);
        Assert.False(result.IsApproved); // high-value rule: LTV > 60 %
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SPEC TEST CASES (sections 18)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void SpecTest1_Loan500k_Asset1M_Score800_Approved()
    {
        var result = _sut.Evaluate(500_000m, 1_000_000m, 800);

        Assert.True(result.IsApproved);
        Assert.Equal(50m, result.Ltv);
        Assert.Equal("Standard LTV < 60%", result.RuleApplied);
    }

    [Fact]
    public void SpecTest2_Loan500k_Asset625k_Score800_Declined_80BandRequires900()
    {
        var result = _sut.Evaluate(500_000m, 625_000m, 800);

        Assert.False(result.IsApproved);
        Assert.Equal(80m, result.Ltv);
        Assert.Equal("Standard LTV 80%-<90%", result.RuleApplied);
        Assert.Contains("900", result.DecisionReason);
    }

    [Fact]
    public void SpecTest3_Loan1M_Asset2M_Score950_Approved_HighValue()
    {
        var result = _sut.Evaluate(1_000_000m, 2_000_000m, 950);

        Assert.True(result.IsApproved);
        Assert.Equal(50m, result.Ltv);
        Assert.Equal("High-value loan rule", result.RuleApplied);
    }

    [Fact]
    public void SpecTest4_Loan1M_Asset2M_Score949_Declined()
    {
        var result = _sut.Evaluate(1_000_000m, 2_000_000m, 949);

        Assert.False(result.IsApproved);
        Assert.Equal("High-value loan rule", result.RuleApplied);
    }

    [Fact]
    public void SpecTest5_Loan50k_Asset100k_Score900_Declined_BelowMinimum()
    {
        var result = _sut.Evaluate(50_000m, 100_000m, 900);

        Assert.False(result.IsApproved);
        Assert.Equal("Minimum loan amount", result.RuleApplied);
        // Reason should mention the minimum amount — we check the rule label, not exact formatting.
        Assert.Contains("minimum", result.DecisionReason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SpecTest6_Loan1500k_Asset2M_Score999_Declined_LTVAbove60()
    {
        // High-value loan: LTV = 75 % > 60 % → declined
        var result = _sut.Evaluate(1_500_000m, 2_000_000m, 999);

        Assert.False(result.IsApproved);
        Assert.Equal(75m, result.Ltv);
        Assert.Equal("High-value loan rule", result.RuleApplied);
    }

    [Fact]
    public void SpecTest7_Loan1500001_Asset2M_Score999_Declined_ExceedsMaximum()
    {
        var result = _sut.Evaluate(1_500_001m, 2_000_000m, 999);

        Assert.False(result.IsApproved);
        Assert.Equal("Maximum loan amount", result.RuleApplied);
        Assert.Contains("maximum", result.DecisionReason, StringComparison.OrdinalIgnoreCase);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CREDIT SCORE THRESHOLDS — just below, at, and just above each threshold
    // ─────────────────────────────────────────────────────────────────────────

    // 750 threshold (LTV < 60 %)
    [Fact]
    public void CreditScore_749_Fails_750Threshold()
    {
        var result = _sut.Evaluate(300_000m, 1_000_000m, 749); // LTV = 30 %

        Assert.False(result.IsApproved);
    }

    [Fact]
    public void CreditScore_750_Passes_750Threshold()
    {
        var result = _sut.Evaluate(300_000m, 1_000_000m, 750); // LTV = 30 %

        Assert.True(result.IsApproved);
    }

    [Fact]
    public void CreditScore_751_Passes_750Threshold()
    {
        var result = _sut.Evaluate(300_000m, 1_000_000m, 751);

        Assert.True(result.IsApproved);
    }

    // 800 threshold (60 % ≤ LTV < 80 %)
    [Fact]
    public void CreditScore_799_Fails_800Threshold()
    {
        var (loan, asset) = ForLtv(70m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 799);

        Assert.False(result.IsApproved);
    }

    [Fact]
    public void CreditScore_800_Passes_800Threshold()
    {
        var (loan, asset) = ForLtv(70m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 800);

        Assert.True(result.IsApproved);
    }

    [Fact]
    public void CreditScore_801_Passes_800Threshold()
    {
        var (loan, asset) = ForLtv(70m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 801);

        Assert.True(result.IsApproved);
    }

    // 900 threshold (80 % ≤ LTV < 90 %)
    [Fact]
    public void CreditScore_899_Fails_900Threshold()
    {
        var (loan, asset) = ForLtv(85m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 899);

        Assert.False(result.IsApproved);
    }

    [Fact]
    public void CreditScore_900_Passes_900Threshold()
    {
        var (loan, asset) = ForLtv(85m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 900);

        Assert.True(result.IsApproved);
    }

    [Fact]
    public void CreditScore_901_Passes_900Threshold()
    {
        var (loan, asset) = ForLtv(85m, 500_000m);
        var result = _sut.Evaluate(loan, asset, 901);

        Assert.True(result.IsApproved);
    }

    // 950 threshold (high-value)
    [Fact]
    public void CreditScore_949_Fails_950Threshold_HighValue()
    {
        var result = _sut.Evaluate(1_000_000m, 2_000_000m, 949);

        Assert.False(result.IsApproved);
    }

    [Fact]
    public void CreditScore_950_Passes_950Threshold_HighValue()
    {
        var result = _sut.Evaluate(1_000_000m, 2_000_000m, 950);

        Assert.True(result.IsApproved);
    }

    [Fact]
    public void CreditScore_951_Passes_950Threshold_HighValue()
    {
        var result = _sut.Evaluate(1_000_000m, 2_000_000m, 951);

        Assert.True(result.IsApproved);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // DECISION RESULT — general
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void Decision_IsApproved_Returns_ApprovedString()
    {
        var result = _sut.Evaluate(500_000m, 1_000_000m, 800);

        Assert.Equal("Approved", result.Decision);
    }

    [Fact]
    public void Decision_IsDeclined_Returns_DeclinedString()
    {
        var result = _sut.Evaluate(50_000m, 1_000_000m, 999);

        Assert.Equal("Declined", result.Decision);
    }

    [Fact]
    public void DecisionReason_IsNotEmpty()
    {
        var result = _sut.Evaluate(500_000m, 1_000_000m, 800);

        Assert.False(string.IsNullOrWhiteSpace(result.DecisionReason));
    }

    [Fact]
    public void RuleApplied_IsNotEmpty()
    {
        var result = _sut.Evaluate(500_000m, 1_000_000m, 800);

        Assert.False(string.IsNullOrWhiteSpace(result.RuleApplied));
    }
}
