using OfficeOpenXml;
using System;

namespace BlazorSimuladorJGF.Models
{
    /// <summary>
    /// Clase que genera un Excel a partir de los resultados de los emails
    /// </summary>
    public class ExcelReportGenerator
    {
        /// <summary>
        /// Genera un reporte en formato Excel a partir de los resultados de una campaña de phishing.
        /// </summary>
        /// <param name="report">El reporte de la campaña de phishing</param>
        /// <returns>Un arreglo de bytes que representa el archivo Excel generado</returns>
        public byte[] GenerateExcelReport(PhishingReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report), "El reporte no puede ser nulo.");
            }

            if (report.DailyStats == null || report.DailyStats.Count == 0)
            {
                throw new ArgumentException("El reporte no contiene estadísticas diarias.", nameof(report));
            }

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Report");

            // Añade la cabecera en Excel
            worksheet.Cells[1, 1].Value = "Date";
            worksheet.Cells[1, 2].Value = "Emails Opened";
            worksheet.Cells[1, 3].Value = "Links Clicked";

            // Añade los datos a las filas
            for (int i = 0; i < report.DailyStats.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = report.DailyStats[i].Date.ToString("yyyy-MM-dd");
                worksheet.Cells[i + 2, 2].Value = report.DailyStats[i].EmailsOpened;
                worksheet.Cells[i + 2, 3].Value = report.DailyStats[i].LinksClicked;
            }

            // Añade las columnas
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return package.GetAsByteArray();
        }
    }
}
