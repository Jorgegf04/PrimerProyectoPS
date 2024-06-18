using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorSimuladorJGF.Models
{
    public class Campaign
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(100, ErrorMessage = "The Name field cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Template Name field is required.")]
        [StringLength(100, ErrorMessage = "The Template Name field cannot exceed 100 characters.")]
        public string TemplateName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Start Date field is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "The End Date field is required.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.UtcNow;

        public List<Recipient> Recipients { get; set; } = new List<Recipient>();

        // Método opcional para agregar un recipient
        public void AddRecipient(Recipient recipient)
        {
            Recipients.Add(recipient);
        }
    }
}