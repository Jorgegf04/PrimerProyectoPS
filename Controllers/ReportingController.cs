using BlazorSimuladorJGF.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReportingController : ControllerBase
{
    private readonly ReportingService _reportingService;
    private readonly PdfReportGenerator _pdfReportGenerator;
    private readonly ExcelReportGenerator _excelReportGenerator;

    public ReportingController(ReportingService reportingService, PdfReportGenerator pdfReportGenerator, ExcelReportGenerator excelReportGenerator)
    {
        _reportingService = reportingService;
        _pdfReportGenerator = pdfReportGenerator;
        _excelReportGenerator = excelReportGenerator;
    }

    [HttpGet("campaign/{campaignId}/report")]
    public async Task<IActionResult> GetPhishingReport(int campaignId)
    {
        try
        {
            var report = await _reportingService.GeneratePhishingReportAsync(campaignId);
            report.Id = campaignId; // Set the Id property to the campaignId for demonstration
            return Ok(report);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("campaign/{campaignId}/report/pdf")]
    public async Task<IActionResult> GetPdfReport(int campaignId)
    {
        try
        {
            var report = await _reportingService.GeneratePhishingReportAsync(campaignId);
            var pdfBytes = _pdfReportGenerator.GeneratePdfReport(report);
            return File(pdfBytes, "application/pdf", "report.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("campaign/{campaignId}/report/excel")]
    public async Task<IActionResult> GetExcelReport(int campaignId)
    {
        try
        {
            var report = await _reportingService.GeneratePhishingReportAsync(campaignId);
            var excelBytes = _excelReportGenerator.GenerateExcelReport(report);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}