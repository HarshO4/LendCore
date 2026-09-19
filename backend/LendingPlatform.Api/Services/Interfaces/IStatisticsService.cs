using LendingPlatform.Api.Models.DTOs;

namespace LendingPlatform.Api.Services.Interfaces;

/// <summary>
/// Computes portfolio statistics from persisted application data.
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// Return aggregate statistics for all valid persisted applications.
    /// </summary>
    Task<StatisticsResponse> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
