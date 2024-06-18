using BlazorSimuladorJGF.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

[Route("api/[controller]")]
[ApiController]
public class PhishingController : ControllerBase
{
    private readonly IPhishingInterface _phishingService;
    private readonly ILogger<PhishingController> _logger;

    public PhishingController(IPhishingInterface phishingService, ILogger<PhishingController> logger)
    {
        _phishingService = phishingService;
        _logger = logger;
    }

    /// <summary>
    /// Tracks email open event.
    /// </summary>
    /// <param name="emailId">The ID of the email.</param>
    /// <returns>An empty result.</returns>
    [HttpGet("open")]
    public async Task<IActionResult> TrackOpen(string emailId)
    {
        if (string.IsNullOrEmpty(emailId))
        {
            _logger.LogWarning("Email ID is null or empty.");
            return BadRequest("Email ID cannot be null or empty.");
        }

        try
        {
            bool result = await _phishingService.MarkEmailAsOpenedAsync(emailId);
            if (result)
            {
                _logger.LogInformation("Email open tracked successfully for emailId: {EmailId}", emailId);
                return Ok();
            }
            else
            {
                _logger.LogWarning("Failed to track email open for emailId: {EmailId}", emailId);
                return StatusCode(500, "Error tracking email open.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking email open for emailId: {EmailId}", emailId);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Tracks email click event and redirects to the target URL.
    /// </summary>
    /// <param name="emailId">The ID of the email.</param>
    /// <param name="target">The target URL to redirect to.</param>
    /// <returns>A redirection to the target URL.</returns>
    [HttpGet("click")]
    public async Task<IActionResult> TrackClick(string emailId, string target)
    {
        if (string.IsNullOrEmpty(emailId))
        {
            _logger.LogWarning("Email ID is null or empty.");
            return BadRequest("Email ID cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(target))
        {
            _logger.LogWarning("Target URL is null or empty.");
            return BadRequest("Target URL cannot be null or empty.");
        }

        try
        {
            bool result = await _phishingService.MarkEmailAsClickedAsync(emailId);
            if (result)
            {
                _logger.LogInformation("Email click tracked successfully for emailId: {EmailId}, redirecting to: {Target}", emailId, target);
                return Redirect(target);
            }
            else
            {
                _logger.LogWarning("Failed to track email click for emailId: {EmailId}", emailId);
                return StatusCode(500, "Error tracking link click.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking email click for emailId: {EmailId}", emailId);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}