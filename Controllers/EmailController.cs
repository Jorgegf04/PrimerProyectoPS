using BlazorSimuladorJGF.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly ICampaignInterface _campaignService;
    private readonly ILogger<EmailController> _logger;

    public EmailController(ICampaignInterface campaignService, ILogger<EmailController> logger)
    {
        _campaignService = campaignService;
        _logger = logger;
    }

    /// <summary>
    /// Rastrea el evento de apertura de un email.
    /// </summary>
    /// <param name="emailId">El ID del email.</param>
    /// <returns>Resultado de la acción.</returns>
    [HttpGet("open")]
    public async Task<IActionResult> TrackOpen(string emailId)
    {
        try
        {
            await _campaignService.MarkEmailAsOpenedAsync(emailId);
            _logger.LogInformation("Email open tracked for emailId: {EmailId}", emailId);
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking email open for emailId: {EmailId}", emailId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Rastrea el evento de clic en un email y redirige a la URL de destino.
    /// </summary>
    /// <param name="emailId">El ID del email.</param>
    /// <param name="target">La URL de destino a la que redirigir.</param>
    /// <returns>Resultado de la acción.</returns>
    [HttpGet("click")]
    public async Task<IActionResult> TrackClick(string emailId, string target)
    {
        try
        {
            await _campaignService.MarkEmailAsClickedAsync(emailId);
            _logger.LogInformation("Email click tracked for emailId: {EmailId}, redirecting to: {Target}", emailId, target);
            return Redirect(target);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking email click for emailId: {EmailId}", emailId);
            return StatusCode(500, "Internal server error");
        }
    }
}