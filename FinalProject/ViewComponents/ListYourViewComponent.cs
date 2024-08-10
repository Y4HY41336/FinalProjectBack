using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.List;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewComponents;

public class ListYourViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public ListYourViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var CurrentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
        var lists = await _context.UserLists.ToListAsync();
        var followedLists = await _context.ListFollowers.Where(l => l.UserId == CurrentUser.Id).ToListAsync();

        var followedListIds = followedLists.Select(f => f.ListId).ToList();

        var listsToFollow = lists.Where(l => !l.IsDeleted)
                                 .Where(l => followedListIds.Contains(l.Id) || l.OwnerId == CurrentUser.Id)
                                 .OrderByDescending(l => l.OwnerId == CurrentUser.Id)
                                 .ToList();

        ListDiscoverViewModel model = new()
        {
            Lists = listsToFollow,
            FollowedLists = followedLists,
            CurrentUser = CurrentUser,
        };
        return View(model);
    }
}
