using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers;

public class NotificationsController : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        ViewBag.TabId = HttpContext.Session.GetString("NotificationTabId") ?? "All";
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Filter(string? TabId)
    {
        return ViewComponent("Notification", new { TabId = TabId });
    }

}
