using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BlazorSimuladorJGF.Models;

namespace BlazorSimuladorJGF.Services
{
    /// <summary>
    /// Servicio para gestionar las pruebas de phishing.
    /// </summary>
    public class PhishingService : IPhishingInterface
    {
        private readonly string _connectionString;
        private readonly ILogger<PhishingService> _logger;

        /// <summary>
        /// Constructor del servicio de phishing.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación.</param>
        /// <param name="logger">Instancia del registrador de logs.</param>
        public PhishingService(IConfiguration configuration, ILogger<PhishingService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        /// <summary>
        /// Registra un intento de phishing.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="campaignId">ID de la campaña.</param>
        /// <param name="isPhished">Indica si el usuario fue phishado.</param>
        /// <returns>Verdadero si el registro fue exitoso; de lo contrario, falso.</returns>
        public async Task<bool> LogPhishingAttemptAsync(int userId, int campaignId, bool isPhished)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("INSERT INTO PhishingTests (UserId, CampaignId, IsPhished, PhishedAt) VALUES (@UserId, @CampaignId, @IsPhished, @PhishedAt)", connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@CampaignId", campaignId);
                        command.Parameters.AddWithValue("@IsPhished", isPhished);
                        command.Parameters.AddWithValue("@PhishedAt", DateTime.Now);

                        var result = await command.ExecuteNonQueryAsync();
                        _logger.LogInformation("Phishing attempt logged for user {UserId} in campaign {CampaignId}", userId, campaignId);
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging phishing attempt for user {UserId} in campaign {CampaignId}", userId, campaignId);
                throw new Exception("Error logging phishing attempt.", ex);
            }
        }

        /// <summary>
        /// Recupera los resultados de phishing para una campaña específica.
        /// </summary>
        /// <param name="campaignId">ID de la campaña.</param>
        /// <returns>Lista de resultados de phishing.</returns>
        public async Task<List<PhishingTest>> GetPhishingResultsAsync(int campaignId)
        {
            var results = new List<PhishingTest>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SELECT Id, UserId, CampaignId, IsPhished, PhishedAt FROM PhishingTests WHERE CampaignId = @CampaignId", connection))
                    {
                        command.Parameters.AddWithValue("@CampaignId", campaignId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new PhishingTest
                                {
                                    Id = reader.GetInt32(0),
                                    UserId = reader.GetInt32(1),
                                    CampaignId = reader.GetInt32(2),
                                    IsPhished = reader.GetBoolean(3),
                                    PhishedAt = reader.GetDateTime(4)
                                });
                            }
                        }
                    }
                }
                _logger.LogInformation("Phishing results retrieved for campaign {CampaignId}", campaignId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving phishing results for campaign {CampaignId}", campaignId);
                throw new Exception("Error retrieving phishing results.", ex);
            }

            return results;
        }

        /// <summary>
        /// Marca un correo electrónico como abierto.
        /// </summary>
        /// <param name="emailId">ID del correo electrónico.</param>
        /// <returns>Verdadero si la actualización fue exitosa; de lo contrario, falso.</returns>
        public async Task<bool> MarkEmailAsOpenedAsync(string emailId)
        {
            return await UpdateEmailStatusAsync(emailId, "Opened", "OpenedDateTime");
        }

        /// <summary>
        /// Marca un correo electrónico como clickeado.
        /// </summary>
        /// <param name="emailId">ID del correo electrónico.</param>
        /// <returns>Verdadero si la actualización fue exitosa; de lo contrario, falso.</returns>
        public async Task<bool> MarkEmailAsClickedAsync(string emailId)
        {
            return await UpdateEmailStatusAsync(emailId, "Clicked", "ClickedDateTime");
        }

        /// <summary>
        /// Actualiza el estado de un correo electrónico.
        /// </summary>
        /// <param name="emailId">ID del correo electrónico.</param>
        /// <param name="statusColumn">Columna de estado a actualizar.</param>
        /// <param name="dateColumn">Columna de fecha a actualizar.</param>
        /// <returns>Verdadero si la actualización fue exitosa; de lo contrario, falso.</returns>
        private async Task<bool> UpdateEmailStatusAsync(string emailId, string statusColumn, string dateColumn)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand($"UPDATE EmailResults SET {statusColumn} = 1, {dateColumn} = @DateTime WHERE Id = @EmailId", connection))
                    {
                        command.Parameters.AddWithValue("@DateTime", DateTime.Now);
                        command.Parameters.AddWithValue("@EmailId", emailId);

                        var result = await command.ExecuteNonQueryAsync();
                        _logger.LogInformation("Email marked as {Status} with ID {EmailId}", statusColumn.ToLower(), emailId);
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking email as {Status} with ID {EmailId}", statusColumn.ToLower(), emailId);
                throw new Exception($"Error marking email as {statusColumn.ToLower()}.", ex);
            }
        }
    }
}