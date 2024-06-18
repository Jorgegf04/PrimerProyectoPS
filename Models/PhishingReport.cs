namespace BlazorSimuladorJGF.Models
{
    /// <summary>
    /// Clase que representa el total de todas las estadísticas 
    /// </summary>
    public class PhishingReport
    {
        /// <summary>
        /// Identificador único para el reporte
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador único para la campaña
        /// </summary>
        public int CampaignId { get; set; }

        /// <summary>
        /// Número total de emails enviados
        /// </summary>
        public int TotalEmailsSent { get; set; }

        /// <summary>
        /// Número total de emails abiertos
        /// </summary>
        public int TotalEmailsOpened { get; set; }

        /// <summary>
        /// Número total de enlaces clickeados
        /// </summary>
        public int TotalLinksClicked { get; set; }

        /// <summary>
        /// Para guardar todos los intentos de los usuarios
        /// </summary>
        public Dictionary<string, int> PhishingAttemptsByUser { get; set; }

        /// <summary>
        /// Para poder generar un listado de las estadísticas diarias
        /// </summary>
        public List<DailyEmailStats> DailyStats { get; set; }

        /// <summary>
        /// Inicializa una instancia de la clase de reportes de PhishingReport
        /// </summary>
        public PhishingReport()
        {
            PhishingAttemptsByUser = new Dictionary<string, int>();
            DailyStats = new List<DailyEmailStats>();
        }
    }
}