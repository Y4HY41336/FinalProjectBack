using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.List;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewComponents;

public class ListFollowersViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public ListFollowersViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] int listId)
    {
        ViewBag.ListId = listId;

        var CurrentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
        var users = await _context.Users.Where(u => u.IsActive).ToListAsync();

        var follow = await _context.Follows.Where(f => f.FollowerId == CurrentUser.Id).ToListAsync();

        var followers = await _context.ListFollowers.Where(m => m.ListId == listId).ToListAsync();

        var addedUserIds = followers.Select(f => f.UserId).ToList();

        var membersToAdd = users.Where(u => addedUserIds.Contains(u.Id)).ToList();


        ListMembersViewModel model = new()
        {
            Users = membersToAdd,
            Followers = followers,
            CurrentUser = CurrentUser,
            Follow = follow,
        };
        return View(model);
    }
}
