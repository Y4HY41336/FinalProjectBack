using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.LayoutViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewComponents;

public class ShowFollowViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public ShowFollowViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] string? TabId)
    {
        var CurrentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
        var users = await _context.Users.Where(u => u.Id != CurrentUser.Id).ToListAsync();
        var follows = await _context.Follows.Where(f => f.FollowerId == CurrentUser.Id).ToListAsync();

        var followedUserIds = follows.Select(f => f.FollowedId).ToList();

        var usersToFollow = users.Where(u => !followedUserIds.Contains(u.Id)).ToList();

        if (TabId == "Follow")
        {

        }
        else if (TabId == "Creators")
        {

        }
        else
        {

        }

        FollowUserViewModel model = new()
        {
            Users = usersToFollow,
            Follows = follows,
            CurrentUser = CurrentUser,
            TabId = TabId,
        };
        return View(model);
    }
}
