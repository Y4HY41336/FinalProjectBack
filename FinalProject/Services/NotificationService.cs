using FinalProject.Context;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Services;

public class NotificationService
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public NotificationService(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task SendNotificationAsync(string userId, string receiverId, string type, string message = "")
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.UserId == userId && n.ReceiverId == receiverId && n.Type == type);
        if (notification != null)
        {
            
        }
        else
        {
            var newNotification = new Notification
            {
                UserId = userId,
                ReceiverId = receiverId,
                PostId = 420,
                Type = type,
                Message = message,
                CreatedDate = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(newNotification);
            await _context.SaveChangesAsync();
        }        
    }

    public async Task SendNotificationAsync(string userId, string receiverId, int postId, string type, string message = "")
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.UserId == userId && n.ReceiverId == receiverId && n.PostId == postId && n.Type == type);
        if (notification != null)
        {

        }
        else
        {
            var newNotification = new Notification
            {
                UserId = userId,
                ReceiverId = receiverId,
                PostId = postId,
                Type = type,
                Message = message,
                CreatedDate = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(newNotification);
            await _context.SaveChangesAsync();
        }        
    }
}
