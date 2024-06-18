using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorSimuladorJGF.Models
{
    /// <summary>
    /// Representa a un usuario de la aplicación
    /// </summary>
    public class User
    {
        /// <summary>
        /// Identificador único para los usuarios
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// El email del usuario
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// El nombre del usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Valor para representar si un usuario ha sido phiseado o no
        /// </summary>
        public bool IsPhished { get; set; } = false;

        /// <summary>
        /// Lista de los tests de phishing del usuario
        /// </summary>
        public List<PhishingTest> PhishingTests { get; set; } = new List<PhishingTest>();
    }
}