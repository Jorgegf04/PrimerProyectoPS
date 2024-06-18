using System.ComponentModel.DataAnnotations;

namespace BlazorSimuladorJGF.Models
{
    /// <summary>
    /// Clase que representa el template para una campaña
    /// </summary>
    public class EmailTemplate
    {
        /// <summary>
        /// Atributo de identificador unico para los templates
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Atributo para el nombre del template
        /// </summary>
        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(100, ErrorMessage = "The Name field cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Atributo que representa el sujeto del template
        /// </summary>
        [Required(ErrorMessage = "The Subject field is required.")]
        [StringLength(200, ErrorMessage = "The Subject field cannot exceed 200 characters.")]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Atributo que representa el cuerpo del template
        /// </summary>
        [Required(ErrorMessage = "The Body field is required.")]
        public string Body { get; set; } = string.Empty;

        public bool IsSuspicious { get; set; }
    }
}
