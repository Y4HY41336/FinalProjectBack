using FinalProject.Models;

namespace FinalProject.ViewModels.Explore
{
    public class ExploreTrendsViewModel
    {
        public List<Trend> Trends { get; set; } = null!;
        public string? TabId { get; set; }
    }
}
