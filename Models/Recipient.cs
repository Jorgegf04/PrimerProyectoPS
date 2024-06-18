using System.ComponentModel.DataAnnotations;

namespace BlazorSimuladorJGF.Models
{
    /// <summary>
    /// Representa el recipiente de una campaña
    /// </summary>
    public class Recipient
    {
        /// <summary>
        /// Identificador único para los recipientes
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Identificador de la campaña asociada
        /// </summary>
        [Required]
        public int CampaignId { get; set; }

        /// <summary>
        /// El email del recipiente
        /// </summary>
        [Required]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Campaña asociada con el recipiente
        /// </summary>
        public Campaign? Campaign { get; set; }  // Propiedad de navegación opcional

        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(100, ErrorMessage = "The Name field cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}