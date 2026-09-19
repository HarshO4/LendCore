using LendingPlatform.Api.Models.DTOs;
using LendingPlatform.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LendingPlatform.Api.Controllers;

/// <summary>
/// Returns portfolio statistics calculated from persisted application data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// Retrieve portfolio statistics across all persisted applications.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Aggregate portfolio statistics.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(StatisticsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
    {
        var statistics = await _statisticsService.GetStatisticsAsync(cancellationToken);
        return Ok(statistics);
    }
}
