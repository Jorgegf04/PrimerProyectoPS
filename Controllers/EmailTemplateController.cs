using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorSimuladorJGF.Models;
using BlazorSimuladorJGF.Services;
using System;
using Microsoft.Extensions.Logging;

[Route("api/[controller]")]
[ApiController]
public class EmailTemplateController : ControllerBase
{
    private readonly EmailTemplateService _emailTemplateService;
    private readonly ILogger<EmailTemplateController> _logger;

    public EmailTemplateController(EmailTemplateService emailTemplateService, ILogger<EmailTemplateController> logger)
    {
        _emailTemplateService = emailTemplateService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all email templates.
    /// </summary>
    /// <returns>Action result containing the list of email templates.</returns>
    [HttpGet]
    public async Task<IActionResult> GetTemplates()
    {
        try
        {
            var templates = await _emailTemplateService.GetTemplatesAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving email templates");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new email template.
    /// </summary>
    /// <param name="template">The email template to create.</param>
    /// <returns>Action result.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateTemplate([FromBody] EmailTemplate template)
    {
        if (template == null)
        {
            _logger.LogWarning("Attempted to create a null email template.");
            return BadRequest("Template cannot be null.");
        }

        try
        {
            await _emailTemplateService.CreateTemplateAsync(template);
            return Ok("Email template created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating email template: {TemplateName}", template.Name);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing email template.
    /// </summary>
    /// <param name="template">The email template to update.</param>
    /// <returns>Action result.</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateTemplate([FromBody] EmailTemplate template)
    {
        if (template == null)
        {
            _logger.LogWarning("Attempted to update a null email template.");
            return BadRequest("Template cannot be null.");
        }

        try
        {
            await _emailTemplateService.UpdateTemplateAsync(template);
            return Ok("Email template updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating email template: {TemplateName}", template.Name);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes an email template by ID.
    /// </summary>
    /// <param name="id">The ID of the email template to delete.</param>
    /// <returns>Action result.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate(int id)
    {
        try
        {
            await _emailTemplateService.DeleteTemplateAsync(id);
            return Ok("Email template deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting email template with ID: {TemplateId}", id);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}