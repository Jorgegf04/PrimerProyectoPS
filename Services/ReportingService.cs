using BlazorSimuladorJGF.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class ReportingService
{
    private readonly string _connectionString;
    private readonly ILogger<ReportingService> _logger;
    private readonly PdfReportGenerator _pdfReportGenerator;
    private readonly ExcelReportGenerator _excelReportGenerator;

    public ReportingService(IConfiguration configuration, ILogger<ReportingService> logger, PdfReportGenerator pdfReportGenerator, ExcelReportGenerator excelReportGenerator)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
        _pdfReportGenerator = pdfReportGenerator;
        _excelReportGenerator = excelReportGenerator;
    }

    public async Task<PhishingReport> GeneratePhishingReportAsync(int campaignId)
    {
        var report = new PhishingReport { CampaignId = campaignId };

        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM EmailResults WHERE CampaignId = @CampaignId", connection))
                {
                    command.Parameters.AddWithValue("@CampaignId", campaignId);
                    report.TotalEmailsSent = (int)await command.ExecuteScalarAsync();
                }

                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM EmailResults WHERE CampaignId = @CampaignId AND Opened = 1", connection))
                {
                    command.Parameters.AddWithValue("@CampaignId", campaignId);
                    report.TotalEmailsOpened = (int)await command.ExecuteScalarAsync();
                }

                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM EmailResults WHERE CampaignId = @CampaignId AND Clicked = 1", connection))
                {
                    command.Parameters.AddWithValue("@CampaignId", campaignId);
                    report.TotalLinksClicked = (int)await command.ExecuteScalarAsync();
                }

                using (SqlCommand command = new SqlCommand("SELECT UserId, COUNT(*) FROM PhishingTests WHERE CampaignId = @CampaignId GROUP BY UserId", connection))
                {
                    command.Parameters.AddWithValue("@CampaignId", campaignId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            report.PhishingAttemptsByUser.Add(reader.GetInt32(0).ToString(), reader.GetInt32(1));
                        }
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error while generating phishing report for campaign {CampaignId}", campaignId);
            throw new Exception("Database error while generating phishing report.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while generating phishing report for campaign {CampaignId}", campaignId);
            throw new Exception("Unexpected error while generating phishing report.", ex);
        }

        return report;
    }

    public async Task<byte[]> GetPdfReportBytesAsync(int campaignId)
    {
        var report = await GeneratePhishingReportAsync(campaignId);
        return _pdfReportGenerator.GeneratePdfReport(report);
    }

    public async Task<byte[]> GetExcelReportBytesAsync(int campaignId)
    {
        var report = await GeneratePhishingReportAsync(campaignId);
        return _excelReportGenerator.GenerateExcelReport(report);
    }
}