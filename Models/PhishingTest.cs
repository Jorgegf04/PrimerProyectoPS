using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorSimuladorJGF.Models
{
    /// <summary>
    /// Representa el resultado de un usuario en una campaña de phishing
    /// </summary>
    public class PhishingTest
    {
        /// <summary>
        /// Identificador único para los tests de phishing
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Identificador de la campaña
        /// </summary>
        [Required]
        public int CampaignId { get; set; }

        /// <summary>
        /// Identificador del usuario
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Indica si el usuario ha sido phished
        /// </summary>
        [Required]
        public bool IsPhished { get; set; }

        /// <summary>
        /// Fecha y hora en que el usuario fue phished
        /// </summary>
        public DateTime PhishedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Campaña asociada con el test de phishing
        /// </summary>
        public Campaign? Campaign { get; set; }  // Propiedad de navegación opcional

        /// <summary>
        /// Usuario asociado con el test de phishing
        /// </summary>
        public User? User { get; set; }  // Propiedad de navegación opcional
    }
}
