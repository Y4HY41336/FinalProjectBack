using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewComponents;

public class NotificationViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public NotificationViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] string? TabId)
    {
        if (string.IsNullOrEmpty(TabId))
        {
            TabId = HttpContext.Session.GetString("NotificationTabId") ?? "All";
        }
        else
        {
            HttpContext.Session.SetString("NotificationTabId", TabId);
        }

        var CurrentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

        var notificationQuery = _context.Notifications.Where(n => n.ReceiverId == CurrentUser.Id).Include(p => p.User).AsQueryable();

        List<Notification> notification = new()
        {

        };
        
        switch (TabId)
        {
            case "All":

                notification = await notificationQuery.OrderByDescending(n => n.CreatedDate).ToListAsync();

                break;

            case "Verified":

                notification = await notificationQuery.Where(n => n.User.IsVerified || n.User.IsPremium || n.User.IsGovermentVerified).OrderByDescending(n => n.CreatedDate).ToListAsync();

                break;

            case "Replys":

                notification = await notificationQuery.Where(n => n.Type == "Reply").OrderByDescending(n => n.CreatedDate).ToListAsync();

                break;

            default:
                

                break;
        }       

        foreach (var notificationItem in notification)
        {
            if (!notificationItem.IsRead)
            {
                notificationItem.IsRead = true;
                _context.Notifications.Update(notificationItem);
            }
        }

        await _context.SaveChangesAsync();
        return View(notification);
    }

}

