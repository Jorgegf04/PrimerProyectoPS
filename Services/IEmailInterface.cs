using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorSimuladorJGF.Models;

namespace BlazorSimuladorJGF.Services
{
    public interface IEmailInterface
    {
        Task SendEmailAsync(EmailDTO request);
        EmailTemplate GetTemplateByName(string templateName);
        string ReplacePlaceholders(string templateBody, Dictionary<string, string> placeholders);
        Task SendEmailWithTemplateAsync(string to, string templateName, Dictionary<string, string> placeholders);
    }
}
