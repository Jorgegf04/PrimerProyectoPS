using System;
using System.IO;
using BlazorSimuladorJGF.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BlazorSimuladorJGF.Models
{
    public class PdfReportGenerator
    {
        public byte[] GeneratePdfReport(PhishingReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            using var ms = new MemoryStream();
            var document = new Document();
            PdfWriter.GetInstance(document, ms);
            document.Open();

            // Adding title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            document.Add(new Paragraph("Phishing Campaign Report", titleFont) { Alignment = Element.ALIGN_CENTER });
            document.Add(new Paragraph("\n"));

            // Adding summary
            document.Add(new Paragraph($"Total Emails Sent: {report.TotalEmailsSent}", normalFont));
            document.Add(new Paragraph($"Total Emails Opened: {report.TotalEmailsOpened}", normalFont));
            document.Add(new Paragraph($"Total Links Clicked: {report.TotalLinksClicked}", normalFont));
            document.Add(new Paragraph("\n"));

            // Adding daily statistics
            foreach (var stat in report.DailyStats)
            {
                document.Add(new Paragraph($"{stat.Date.ToString("yyyy-MM-dd")}: {stat.EmailsOpened} emails opened", normalFont));
            }

            document.Close();
            return ms.ToArray();
        }
    }
}
