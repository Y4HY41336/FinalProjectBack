using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers;

public class ExploreController : Controller
{
    [Authorize]
    public IActionResult Index()
    {       
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Filter(string? TabId)
    {
        return ViewComponent("ExploreTrend", new { TabId = TabId });
    }

}
