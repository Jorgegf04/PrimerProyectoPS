using BlazorSimuladorJGF.Models;
using BlazorSimuladorJGF.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

[Route("api/[controller]")]
[ApiController]
public class CampaignController : ControllerBase
{
    private readonly ICampaignInterface _campaignService;
    private readonly ILogger<CampaignController> _logger;

    public CampaignController(ICampaignInterface campaignService, ILogger<CampaignController> logger)
    {
        _campaignService = campaignService;
        _logger = logger;
    }

    /// <summary>
    /// Crea una nueva campaña.
    /// </summary>
    /// <param name="campaign">El objeto campaña que se va a crear.</param>
    /// <returns>Resultado de la acción.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateCampaign(Campaign campaign)
    {
        try
        {
            await _campaignService.CreateCampaignAsync(campaign);
            return Ok("Campaign created and emails sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating campaign.");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Recupera todas las campañas.
    /// </summary>
    /// <returns>Resultado de la acción que contiene la lista de campañas.</returns>
    [HttpGet]
    public async Task<IActionResult> GetCampaigns()
    {
        try
        {
            var campaigns = await _campaignService.GetCampaignsAsync();
            return Ok(campaigns);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving campaigns.");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Actualiza una campaña existente.
    /// </summary>
    /// <param name="campaign">El objeto campaña que se va a actualizar.</param>
    /// <returns>Resultado de la acción.</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateCampaign(Campaign campaign)
    {
        try
        {
            await _campaignService.UpdateCampaignAsync(campaign);
            return Ok("Campaign updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating campaign.");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Elimina una campaña por su ID.
    /// </summary>
    /// <param name="id">El ID de la campaña a eliminar.</param>
    /// <returns>Resultado de la acción.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCampaign(int id)
    {
        try
        {
            await _campaignService.DeleteCampaignAsync(id);
            return Ok("Campaign deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting campaign.");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}