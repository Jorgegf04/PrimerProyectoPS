using System.ComponentModel.DataAnnotations;

namespace BlazorSimuladorJGF.Models
{
    /// <summary>
    /// Clase que representa las partes de un gmail para Mailkit y Minekit
    /// </summary>
    public class EmailDTO
    {
        /// <summary>
        /// Atributo que representa para quien es el email
        /// </summary>
        [Required(ErrorMessage = "The To field is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Atributo que presenta de quien es el email
        /// </summary>
        [Required(ErrorMessage = "The Subject field is required.")]
        [StringLength(200, ErrorMessage = "The Subject cannot exceed 200 characters.")]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Atributo que representa el cuerpo del gmail
        /// </summary>
        [Required(ErrorMessage = "The Body field is required.")]
        public string Body { get; set; } = string.Empty;
    }
}