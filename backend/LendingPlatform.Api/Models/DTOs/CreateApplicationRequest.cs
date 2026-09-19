using System.ComponentModel.DataAnnotations;

namespace LendingPlatform.Api.Models.DTOs;

/// <summary>
/// Request body for submitting a new loan application.
/// </summary>
public class CreateApplicationRequest
{
    /// <summary>Requested loan amount in GBP. Must be greater than 0.</summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Loan amount must be greater than 0.")]
    public decimal LoanAmount { get; set; }

    /// <summary>Value of the asset used as security in GBP. Must be greater than 0.</summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Asset value must be greater than 0.")]
    public decimal AssetValue { get; set; }

    /// <summary>Applicant credit score. Must be between 1 and 999 inclusive.</summary>
    [Required]
    [Range(1, 999, ErrorMessage = "Credit score must be between 1 and 999.")]
    public int CreditScore { get; set; }
}
