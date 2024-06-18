using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorSimuladorJGF.Models;

namespace BlazorSimuladorJGF.Services
{
    public interface ICampaignInterface
    {
        Task CreateCampaignAsync(Campaign campaign);
        Task<List<Campaign>> GetCampaignsAsync();
        Task<Campaign> GetCampaignByIdAsync(int id);  
        Task MarkEmailAsOpenedAsync(string emailId);
        Task MarkEmailAsClickedAsync(string emailId);
        Task UpdateCampaignAsync(Campaign campaign);
        Task DeleteCampaignAsync(int id);
    }
}