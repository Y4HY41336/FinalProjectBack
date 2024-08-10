using FinalProject.Context;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewComponents;

public class FollowedPostViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public FollowedPostViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var CurrentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
        var users = await _context.Users.Where(u => u.Id != CurrentUser.Id).ToListAsync();
        var follows = await _context.Follows.Where(f => f.FollowerId == CurrentUser.Id).ToListAsync();

        var followedUserIds = follows.Select(f => f.FollowedId).ToList();


        var posts = await _context.Posts.Where(p => p.User.IsActive == true)
                                        .Where(p => p.IsActive == true)
                                        .Where(p => p.IsActive && followedUserIds.Contains(p.UserId) || p.UserId == CurrentUser.Id)
                                        .Include(p => p.User)
                                        .OrderByDescending(p => p.CreatedDate)
                                        .ToListAsync();
        return View(posts);
    }
}
