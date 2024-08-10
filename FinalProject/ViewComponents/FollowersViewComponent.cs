using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.HomeViewModels;
using FinalProject.ViewModels.LayoutViewModels;
using FinalProject.ViewModels.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewComponents;

public class FollowersViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public FollowersViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] string? TabId, [FromQuery] string? userId)
    {
        if (string.IsNullOrEmpty(TabId))
        {
            TabId = HttpContext.Session.GetString("FollowersTabId") ?? "VerifiedFollowers";
        }
        else
        {
            HttpContext.Session.SetString("FollowersTabId", TabId);
        }
        if (string.IsNullOrEmpty(userId))
        {
            userId = HttpContext.Session.GetString("FollowersUserId");
        }
        var MainUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

        var CurrentUser = await _userManager.FindByIdAsync(userId);
        var users = await _context.Users.Where(u => u.Id != CurrentUser.Id).ToListAsync();
        var verifiedUsers = await _context.Users.Where(u => u.Id != CurrentUser.Id).Where(u => u.IsVerified || u.IsPremium || u.IsGovermentVerified).ToListAsync();

        var followQuery = _context.Follows.AsQueryable();

        var MainUserFollowing = await followQuery.Where(f => f.FollowerId == MainUser.Id).ToListAsync();
        var MainUserFollowers = await followQuery.Where(f => f.FollowedId == MainUser.Id).ToListAsync();

        var following = await followQuery.Where(f => f.FollowerId == CurrentUser.Id).ToListAsync();
        var followers = await followQuery.Where(f => f.FollowedId == CurrentUser.Id).ToListAsync();

        var followingUserIds = following.Select(f => f.FollowedId).ToList();
        var followersUserIds = followers.Select(f => f.FollowerId).ToList();

        var followingUsers = users.Where(u => followingUserIds.Contains(u.Id)).ToList();
        var followersUsers = users.Where(u => followersUserIds.Contains(u.Id)).ToList();
        var VerifiedUsers = verifiedUsers.Where(u => followersUserIds.Contains(u.Id)).ToList();

        FollowUserViewModel model = new()
        {

        };

        switch (TabId)
        {
            case "VerifiedFollowers":
                
                model = new()
                {
                    Follows = MainUserFollowing,
                    CurrentUser = CurrentUser,
                    Users = VerifiedUsers,
                    TabId = TabId,
                };
                
                break;

            case "Followers":
                model = new()
                {
                    Follows = MainUserFollowing,
                    CurrentUser = CurrentUser,
                    Users = followersUsers,
                    TabId = TabId,
                };

                break;

            case "Following":
                model = new()
                {
                    Follows = MainUserFollowing,
                    CurrentUser = CurrentUser,
                    Users = followingUsers,
                    TabId = TabId,
                };

                break;

            default:
                model = new()
                {
                    Follows = MainUserFollowing,
                    CurrentUser = CurrentUser,
                    Users = followersUsers,
                    TabId = TabId,
                };

                break;
        }

        return View(model);
    }
}
