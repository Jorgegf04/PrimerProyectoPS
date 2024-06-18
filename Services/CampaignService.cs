using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorSimuladorJGF.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BlazorSimuladorJGF.Services
{
    public class CampaignService : ICampaignInterface
    {
        private readonly string _connectionString;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(IConfiguration configuration, ILogger<CampaignService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task CreateCampaignAsync(Campaign campaign)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("INSERT INTO Campaigns (Name, TemplateName, StartDate, EndDate) OUTPUT INSERTED.ID VALUES (@Name, @TemplateName, @StartDate, @EndDate)", connection))
                    {
                        command.Parameters.AddWithValue("@Name", campaign.Name);
                        command.Parameters.AddWithValue("@TemplateName", campaign.TemplateName);
                        command.Parameters.AddWithValue("@StartDate", campaign.StartDate);
                        command.Parameters.AddWithValue("@EndDate", campaign.EndDate);

                        campaign.Id = (int)await command.ExecuteScalarAsync();
                    }

                    foreach (var recipient in campaign.Recipients)
                    {
                        using (SqlCommand recipientCommand = new SqlCommand("INSERT INTO Recipients (CampaignId, Email, Name) VALUES (@CampaignId, @Email, @Name)", connection))
                        {
                            recipientCommand.Parameters.AddWithValue("@CampaignId", campaign.Id);
                            recipientCommand.Parameters.AddWithValue("@Email", recipient.Email);
                            recipientCommand.Parameters.AddWithValue("@Name", recipient.Name);

                            await recipientCommand.ExecuteNonQueryAsync();
                        }
                    }

                    _logger.LogInformation("Campaign '{CampaignName}' created successfully", campaign.Name);
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while creating campaign '{CampaignName}'", campaign.Name);
                throw new Exception("Database error while creating campaign.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating campaign '{CampaignName}'", campaign.Name);
                throw new Exception("Unexpected error while creating campaign.", ex);
            }
        }

        public async Task<List<Campaign>> GetCampaignsAsync()
        {
            var campaigns = new List<Campaign>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("SELECT Id, Name, TemplateName, StartDate, EndDate FROM Campaigns", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                campaigns.Add(new Campaign
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    TemplateName = reader.GetString(2),
                                    StartDate = reader.GetDateTime(3),
                                    EndDate = reader.GetDateTime(4)
                                });
                            }
                        }
                    }
                }
                _logger.LogInformation("Campaigns retrieved successfully from the database");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while retrieving campaigns");
                throw new Exception("Database error while retrieving campaigns.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving campaigns");
                throw new Exception("Unexpected error while retrieving campaigns.", ex);
            }

            return campaigns;
        }

        public async Task<Campaign> GetCampaignByIdAsync(int id)
        {
            Campaign campaign = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("SELECT Id, Name, TemplateName, StartDate, EndDate FROM Campaigns WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                campaign = new Campaign
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    TemplateName = reader.GetString(2),
                                    StartDate = reader.GetDateTime(3),
                                    EndDate = reader.GetDateTime(4)
                                };
                            }
                        }
                    }

                    if (campaign != null)
                    {
                        using (SqlCommand recipientCommand = new SqlCommand("SELECT Id, Email, Name FROM Recipients WHERE CampaignId = @CampaignId", connection))
                        {
                            recipientCommand.Parameters.AddWithValue("@CampaignId", campaign.Id);

                            using (SqlDataReader recipientReader = await recipientCommand.ExecuteReaderAsync())
                            {
                                while (await recipientReader.ReadAsync())
                                {
                                    campaign.Recipients.Add(new Recipient
                                    {
                                        Id = recipientReader.GetInt32(0),
                                        Email = recipientReader.GetString(1),
                                        Name = recipientReader.GetString(2)
                                    });
                                }
                            }
                        }
                    }

                }
                _logger.LogInformation("Campaign with ID {Id} retrieved successfully", id);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while retrieving campaign with ID {Id}", id);
                throw new Exception("Database error while retrieving campaign.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving campaign with ID {Id}", id);
                throw new Exception("Unexpected error while retrieving campaign.", ex);
            }

            return campaign;
        }

        public async Task MarkEmailAsOpenedAsync(string emailId)
        {
            await UpdateEmailStatusAsync(emailId, "Opened", "OpenedDateTime");
        }

        public async Task MarkEmailAsClickedAsync(string emailId)
        {
            await UpdateEmailStatusAsync(emailId, "Clicked", "ClickedDateTime");
        }

        private async Task UpdateEmailStatusAsync(string emailId, string statusColumn, string dateColumn)
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

                        await command.ExecuteNonQueryAsync();
                        _logger.LogInformation("Email marked as {Status} with ID {EmailId}", statusColumn.ToLower(), emailId);
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error marking email as {Status} with ID {EmailId}", statusColumn.ToLower(), emailId);
                throw new Exception($"Error marking email as {statusColumn.ToLower()}.", ex);
            }
        }

        public async Task UpdateCampaignAsync(Campaign campaign)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("UPDATE Campaigns SET Name = @Name, TemplateName = @TemplateName, StartDate = @StartDate, EndDate = @EndDate WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Name", campaign.Name);
                        command.Parameters.AddWithValue("@TemplateName", campaign.TemplateName);
                        command.Parameters.AddWithValue("@StartDate", campaign.StartDate);
                        command.Parameters.AddWithValue("@EndDate", campaign.EndDate);
                        command.Parameters.AddWithValue("@Id", campaign.Id);

                        await command.ExecuteNonQueryAsync();
                    }

                    using (SqlCommand deleteRecipientsCommand = new SqlCommand("DELETE FROM Recipients WHERE CampaignId = @CampaignId", connection))
                    {
                        deleteRecipientsCommand.Parameters.AddWithValue("@CampaignId", campaign.Id);
                        await deleteRecipientsCommand.ExecuteNonQueryAsync();
                    }

                    foreach (var recipient in campaign.Recipients)
                    {
                        using (SqlCommand recipientCommand = new SqlCommand("INSERT INTO Recipients (CampaignId, Email, Name) VALUES (@CampaignId, @Email, @Name)", connection))
                        {
                            recipientCommand.Parameters.AddWithValue("@CampaignId", campaign.Id);
                            recipientCommand.Parameters.AddWithValue("@Email", recipient.Email);
                            recipientCommand.Parameters.AddWithValue("@Name", recipient.Name);

                            await recipientCommand.ExecuteNonQueryAsync();
                        }
                    }

                    _logger.LogInformation("Campaign '{CampaignName}' updated successfully", campaign.Name);
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while updating campaign '{CampaignName}'", campaign.Name);
                throw new Exception("Database error while updating campaign.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating campaign '{CampaignName}'", campaign.Name);
                throw new Exception("Unexpected error while updating campaign.", ex);
            }
        }

        public async Task DeleteCampaignAsync(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("DELETE FROM Recipients WHERE CampaignId = @CampaignId", connection))
                    {
                        command.Parameters.AddWithValue("@CampaignId", id);
                        await command.ExecuteNonQueryAsync();
                    }

                    using (SqlCommand command = new SqlCommand("DELETE FROM Campaigns WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        await command.ExecuteNonQueryAsync();
                    }
                    _logger.LogInformation("Campaign with ID {CampaignId} deleted successfully", id);
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while deleting campaign with ID {CampaignId}", id);
                throw new Exception("Database error while deleting campaign.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting campaign with ID {CampaignId}", id);
                throw new Exception("Unexpected error while deleting campaign.", ex);
            }
        }
    }
}