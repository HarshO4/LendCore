using LendingPlatform.Api.Data;
using LendingPlatform.Api.Models.DTOs;
using LendingPlatform.Api.Models.Entities;
using LendingPlatform.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LendingPlatform.Api.Controllers;

/// <summary>
/// Handles loan application submissions and retrieval.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ApplicationsController : ControllerBase
{
    private readonly ILoanDecisionService _decisionService;
    private readonly ApplicationDbContext _db;

    public ApplicationsController(ILoanDecisionService decisionService, ApplicationDbContext db)
    {
        _decisionService = decisionService;
        _db              = db;
    }

    /// <summary>
    /// Submit a loan application and receive the lending decision.
    /// Valid applications (approved or declined) are persisted.
    /// Invalid inputs return HTTP 400 and are NOT persisted.
    /// </summary>
    /// <param name="request">Loan application details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The loan decision with all relevant details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitApplication(
        [FromBody] CreateApplicationRequest request,
        CancellationToken cancellationToken)
    {
        // Model validation (data annotations) is handled automatically.
        // If ModelState is invalid, ASP.NET Core returns 400 before we reach here.

        var decisionResult = _decisionService.Evaluate(
            request.LoanAmount,
            request.AssetValue,
            request.CreditScore);

        var entity = new LoanApplication
        {
            LoanAmount     = request.LoanAmount,
            AssetValue     = request.AssetValue,
            CreditScore    = request.CreditScore,
            Ltv            = decisionResult.Ltv,
            Decision       = decisionResult.Decision,
            DecisionReason = decisionResult.DecisionReason,
            RuleApplied    = decisionResult.RuleApplied,
            CreatedAt      = DateTime.UtcNow
        };

        _db.LoanApplications.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(entity);
        return CreatedAtAction(nameof(GetApplications), new { }, response);
    }

    /// <summary>
    /// Retrieve all persisted loan applications, newest first.
    /// </summary>
    /// <param name="decision">
    /// Optional filter: "Approved" or "Declined".
    /// Omit to return all applications.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of loan applications.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ApplicationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplications(
        [FromQuery] string? decision,
        CancellationToken cancellationToken)
    {
        var query = _db.LoanApplications.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(decision))
        {
            query = query.Where(a => a.Decision == decision);
        }

        var applications = await query
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => MapToResponse(a))
            .ToListAsync(cancellationToken);

        return Ok(applications);
    }

    private static ApplicationResponse MapToResponse(LoanApplication entity) =>
        new()
        {
            Id             = entity.Id,
            LoanAmount     = entity.LoanAmount,
            AssetValue     = entity.AssetValue,
            CreditScore    = entity.CreditScore,
            Ltv            = entity.Ltv,
            Decision       = entity.Decision,
            DecisionReason = entity.DecisionReason,
            RuleApplied    = entity.RuleApplied,
            CreatedAt      = entity.CreatedAt
        };
}
