using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorSimuladorJGF.Models
{
    /// <summary>
    /// Clase que representa las estadísticas diarias de los Emails
    /// </summary>
    public class DailyEmailStats
    {
        /// <summary>
        /// Atributo para la fecha de las estadísticas
        /// </summary>
        [Required(ErrorMessage = "The Date field is required.")]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Atributo para saber los emails enviados 
        /// </summary>
        [Required(ErrorMessage = "The Emails Sent field is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "The Emails Sent field must be a non-negative integer.")]
        public int EmailsSent { get; set; } = 0;

        /// <summary>
        /// Atributo para saber los emails abiertos
        /// </summary>
        [Required(ErrorMessage = "The Emails Opened field is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "The Emails Opened field must be a non-negative integer.")]
        public int EmailsOpened { get; set; } = 0;

        /// <summary>
        /// Atributo para saber cuántas veces un usuario ha clickeado en un enlace
        /// </summary>
        [Required(ErrorMessage = "The Links Clicked field is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "The Links Clicked field must be a non-negative integer.")]
        public int LinksClicked { get; set; } = 0;
    }
}
