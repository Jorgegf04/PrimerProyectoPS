using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorSimuladorJGF.Models
{
    /// <summary>
    /// Clase que representa el resultado de un gmail de una campaña
    /// </summary>
    public class EmailResult
    {
        /// <summary>
        /// Atributo unico para identificar cada resultad
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Atributo que funciona como clave foranea para los recipientes de lo emails
        /// </summary>
        [Required(ErrorMessage = "The RecipientId field is required.")]
        public int RecipientId { get; set; }

        /// <summary>
        /// Atributo para tener los recipientes de lo emails
        /// </summary>
        [Required(ErrorMessage = "The Recipient field is required.")]
        public Recipient Recipient { get; set; }  // Propiedad de navegación

        /// <summary>
        /// Atributo para saber el correo ha sido abierto o no
        /// </summary>
        public bool Opened { get; set; }

        /// <summary>
        /// Atributo para saber si ha sido abierto o no
        /// </summary>
        public bool Clicked { get; set; }

        /// <summary>
        /// Atributo para saber cuando fue abierto el gmail
        /// </summary>
        public DateTime? OpenedDateTime { get; set; }

        /// <summary>
        /// Atributo para saber cuando un link ha sido clickado por el usuario
        /// </summary>
        public DateTime? ClickedDateTime { get; set; }
    }
}