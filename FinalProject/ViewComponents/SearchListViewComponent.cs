using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.List;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewComponents;

public class SearchListViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public SearchListViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync(List<UserList>? Lists)
    {
        var CurrentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
        var lists = await _context.UserLists.Where(l => l.OwnerId != CurrentUser.Id && !l.IsPrivate).ToListAsync();
        var followedLists = await _context.ListFollowers.Where(l => l.UserId == CurrentUser.Id).ToListAsync();

        var followedListIds = followedLists.Select(f => f.ListId).ToList();

        var listsToFollow = Lists;

        ListDiscoverViewModel model = new()
        {
            Lists = listsToFollow,
            FollowedLists = followedLists,
            CurrentUser = CurrentUser,
        };
        return View(model);
    }
}
