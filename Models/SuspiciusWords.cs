using System.Collections.Generic;

namespace BlazorSimuladorJGF.Models
{
    /// <summary>
    /// Clase estática que contiene una lista de palabras sospechosas
    /// </summary>
    public static class SuspiciousWords
    {
        /// <summary>
        /// Lista de palabras sospechosas
        /// </summary>
        public static readonly List<string> Words = new List<string>
        {
            "urgent", "sensitive", "password", "login", "verify", "bank", "account", "security"
        };
    }
}