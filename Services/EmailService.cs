using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using BlazorSimuladorJGF.Models;
using MimeKit.Text;
using System.Linq;

namespace BlazorSimuladorJGF.Services
{
    /// <summary>
    /// Servicio para gestionar el envío de correos electrónicos.
    /// </summary>
    public class EmailService : IEmailInterface
    {
        private readonly string _connectionString;
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private readonly List<EmailTemplate> _templates;

        private const string EmailUserNameConfigKey = "Email:UserName";
        private const string EmailPasswordConfigKey = "Email:PassWord";
        private const string EmailHostConfigKey = "Email:Host";
        private const string EmailPortConfigKey = "Email:Port";

        /// <summary>
        /// Constructor del servicio de email.
        /// </summary>
        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
            _logger = logger;
            _templates = new List<EmailTemplate>
            {
                new EmailTemplate { Name = "Welcome", Subject = "Welcome to our service", Body = "Hello, welcome to our service!" },
                new EmailTemplate { Name = "Goodbye", Subject = "Goodbye from our service", Body = "We're sorry to see you go!" }
            };
        }

        /// <summary>
        /// Envia un email de forma individual a un usuario.
        /// </summary>
        public async Task SendEmailAsync(EmailDTO request)
        {
            ValidateEmailRequest(request);

            var email = new MimeMessage
            {
                From = { MailboxAddress.Parse(_config[EmailUserNameConfigKey]) },
                To = { MailboxAddress.Parse(request.To) },
                Subject = request.Subject,
                Body = new TextPart(TextFormat.Html) { Text = request.Body }
            };

            await SendEmailViaSmtpAsync(email);
            _logger.LogInformation("Email sent successfully to {Recipient}", request.To);
        }

        /// <summary>
        /// Recupera un template según su nombre.
        /// </summary>
        public EmailTemplate GetTemplateByName(string templateName)
        {
            return _templates.FirstOrDefault(t => t.Name.Equals(templateName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Reemplaza los placeholders por los valores en el cuerpo del email.
        /// </summary>
        public string ReplacePlaceholders(string templateBody, Dictionary<string, string> placeholders)
        {
            return placeholders.Aggregate(templateBody, (current, placeholder) => current.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value));
        }

        /// <summary>
        /// Envía un email con un template predefinido y los placeholders con valores.
        /// </summary>
        public async Task SendEmailWithTemplateAsync(string to, string templateName, Dictionary<string, string> placeholders)
        {
            var template = GetTemplateByName(templateName) ?? throw new ArgumentException("Template not found");

            var subject = ReplacePlaceholders(template.Subject, placeholders);
            var body = ReplacePlaceholders(template.Body, placeholders);

            var emailId = Guid.NewGuid().ToString();
            var trackingImage = $"<img src=\"https://your-server.com/api/phishing/open?emailId={emailId}\" style=\"display:none;\" />";
            var trackedBody = body + trackingImage;

            foreach (var placeholder in placeholders)
            {
                var trackedLink = $"https://your-server.com/api/phishing/click?emailId={emailId}&target={Uri.EscapeDataString(placeholder.Value)}";
                trackedBody = trackedBody.Replace(placeholder.Value, trackedLink);
            }

            var email = new MimeMessage
            {
                From = { MailboxAddress.Parse(_config[EmailUserNameConfigKey]) },
                To = { MailboxAddress.Parse(to) },
                Subject = subject,
                Body = new TextPart(TextFormat.Html) { Text = trackedBody }
            };

            await SendEmailViaSmtpAsync(email);
            await SaveEmailResultAsync(emailId, to);
            _logger.LogInformation("Email with template '{TemplateName}' sent to {Recipient}", templateName, to);
        }

        /// <summary>
        /// Envía un email a través de SMTP.
        /// </summary>
        private async Task SendEmailViaSmtpAsync(MimeMessage email)
        {
            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(
                    _config[EmailHostConfigKey],
                    Convert.ToInt32(_config[EmailPortConfigKey]),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(_config[EmailUserNameConfigKey], _config[EmailPasswordConfigKey]);
                await smtp.SendAsync(email);
                _logger.LogInformation("Email sent to {Recipient}", email.To.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP error: {Message}", ex.Message);
                throw;
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        /// <summary>
        /// Guarda el resultado del email en la base de datos.
        /// </summary>
        private async Task SaveEmailResultAsync(string emailId, string recipient)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("INSERT INTO EmailResults (Id, RecipientId, Opened, Clicked) VALUES (@Id, (SELECT Id FROM Recipients WHERE Email = @Email), 0, 0);", connection))
                    {
                        command.Parameters.AddWithValue("@Id", emailId);
                        command.Parameters.AddWithValue("@Email", recipient);
                        await command.ExecuteNonQueryAsync();
                    }

                    _logger.LogInformation("Email result saved for recipient {Recipient}", recipient);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving email result for {Recipient}", recipient);
                throw;
            }
        }

        /// <summary>
        /// Valida los campos de la solicitud de email.
        /// </summary>
        private void ValidateEmailRequest(EmailDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            if (string.IsNullOrEmpty(request.To))
                throw new ArgumentNullException(nameof(request.To), "Recipient cannot be null or empty.");

            if (string.IsNullOrEmpty(request.Subject))
                throw new ArgumentNullException(nameof(request.Subject), "Subject cannot be null or empty.");

            if (string.IsNullOrEmpty(request.Body))
                throw new ArgumentNullException(nameof(request.Body), "Content cannot be null or empty.");
        }
    }
}