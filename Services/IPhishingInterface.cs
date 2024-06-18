using BlazorSimuladorJGF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorSimuladorJGF.Services
{
    public interface IPhishingInterface
    {
        Task<bool> LogPhishingAttemptAsync(int userId, int campaignId, bool isPhished);
        Task<List<PhishingTest>> GetPhishingResultsAsync(int campaignId);
        Task<bool> MarkEmailAsOpenedAsync(string emailId);
        Task<bool> MarkEmailAsClickedAsync(string emailId);
    }
}