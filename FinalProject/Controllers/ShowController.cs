using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers;

public class ShowController : Controller
{
    [Authorize]
    public IActionResult Trends()
    {
        return View();
    }

    [Authorize]
    public IActionResult WhoToFollow()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Filter(string? TabId)
    {
        return ViewComponent("ShowFollow", new { TabId = TabId });
    }
}
