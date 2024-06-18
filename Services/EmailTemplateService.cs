using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorSimuladorJGF.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BlazorSimuladorJGF.Services
{
    /// <summary>
    /// Servicio para gestionar plantillas de correo electrónico.
    /// </summary>
    public class EmailTemplateService
    {
        private readonly string _connectionString;
        private readonly ILogger<EmailTemplateService> _logger;

        /// <summary>
        /// Constructor del servicio de plantillas de correo electrónico.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación.</param>
        /// <param name="logger">Instancia del registrador de logs.</param>
        public EmailTemplateService(IConfiguration configuration, ILogger<EmailTemplateService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        private bool ContainsSuspiciousWords(string text)
        {
            return SuspiciousWords.Words.Any(word => text.Contains(word, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Recupera una plantilla de correo electrónico por su ID.
        /// </summary>
        /// <param name="id">ID de la plantilla.</param>
        /// <returns>La plantilla de correo electrónico.</returns>
        public async Task<EmailTemplate> GetTemplateByIdAsync(int id)
        {
            EmailTemplate template = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SELECT Id, Name, Subject, Body, IsSuspicious FROM EmailTemplates WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                template = new EmailTemplate
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Subject = reader.GetString(2),
                                    Body = reader.GetString(3),
                                    IsSuspicious = reader.GetBoolean(4)
                                };
                            }
                        }
                    }
                }
                _logger.LogInformation("Email template with ID {Id} retrieved successfully", id);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while retrieving email template with ID {Id}", id);
                throw new Exception("Database error while retrieving email template.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving email template with ID {Id}", id);
                throw new Exception("Unexpected error while retrieving email template.", ex);
            }

            return template;
        }

        /// <summary>
        /// Recupera todas las plantillas de correo electrónico.
        /// </summary>
        /// <returns>Lista de plantillas de correo electrónico.</returns>
        public async Task<List<EmailTemplate>> GetTemplatesAsync()
        {
            var templates = new List<EmailTemplate>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SELECT Id, Name, Subject, Body, IsSuspicious FROM EmailTemplates", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                templates.Add(new EmailTemplate
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Subject = reader.GetString(2),
                                    Body = reader.GetString(3),
                                    IsSuspicious = reader.GetBoolean(4)
                                });
                            }
                        }
                    }
                }
                _logger.LogInformation("Email templates retrieved successfully from the database");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while retrieving email templates");
                throw new Exception("Database error while retrieving email templates.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving email templates");
                throw new Exception("Unexpected error while retrieving email templates.", ex);
            }

            return templates;
        }

        /// <summary>
        /// Crea una nueva plantilla de correo electrónico.
        /// </summary>
        /// <param name="template">La plantilla de correo electrónico a crear.</param>
        public async Task CreateTemplateAsync(EmailTemplate template)
        {
            template.IsSuspicious = ContainsSuspiciousWords(template.Body);

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("INSERT INTO EmailTemplates (Name, Subject, Body, IsSuspicious) VALUES (@Name, @Subject, @Body, @IsSuspicious)", connection))
                    {
                        command.Parameters.AddWithValue("@Name", template.Name);
                        command.Parameters.AddWithValue("@Subject", template.Subject);
                        command.Parameters.AddWithValue("@Body", template.Body);
                        command.Parameters.AddWithValue("@IsSuspicious", template.IsSuspicious);

                        await command.ExecuteNonQueryAsync();
                        _logger.LogInformation("Email template '{TemplateName}' created successfully", template.Name);
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while creating email template '{TemplateName}'", template.Name);
                throw new Exception("Database error while creating email template.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating email template '{TemplateName}'", template.Name);
                throw new Exception("Unexpected error while creating email template.", ex);
            }
        }

        /// <summary>
        /// Actualiza una plantilla de correo electrónico existente.
        /// </summary>
        /// <param name="template">La plantilla de correo electrónico a actualizar.</param>
        public async Task UpdateTemplateAsync(EmailTemplate template)
        {
            template.IsSuspicious = ContainsSuspiciousWords(template.Body);

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("UPDATE EmailTemplates SET Name = @Name, Subject = @Subject, Body = @Body, IsSuspicious = @IsSuspicious WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Name", template.Name);
                        command.Parameters.AddWithValue("@Subject", template.Subject);
                        command.Parameters.AddWithValue("@Body", template.Body);
                        command.Parameters.AddWithValue("@IsSuspicious", template.IsSuspicious);
                        command.Parameters.AddWithValue("@Id", template.Id);

                        await command.ExecuteNonQueryAsync();
                        _logger.LogInformation("Email template '{TemplateName}' updated successfully", template.Name);
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while updating email template '{TemplateName}'", template.Name);
                throw new Exception("Database error while updating email template.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating email template '{TemplateName}'", template.Name);
                throw new Exception("Unexpected error while updating email template.", ex);
            }
        }

        /// <summary>
        /// Elimina una plantilla de correo electrónico por su ID.
        /// </summary>
        /// <param name="templateId">ID de la plantilla a eliminar.</param>
        public async Task DeleteTemplateAsync(int templateId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("DELETE FROM EmailTemplates WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", templateId);
                        await command.ExecuteNonQueryAsync();
                        _logger.LogInformation("Email template with ID {TemplateId} deleted successfully", templateId);
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while deleting email template with ID {TemplateId}", templateId);
                throw new Exception("Database error while deleting email template.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting email template with ID {TemplateId}", templateId);
                throw new Exception("Unexpected error while deleting email template.", ex);
            }
        }
    }
}