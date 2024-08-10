using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.List;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewComponents;

public class ListSuggestedViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public ListSuggestedViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] int listId)
    {
        ViewBag.ListId = listId;

        var CurrentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
        var users = await _context.Users.Where(u => u.Id != CurrentUser.Id).ToListAsync();

        var members = await _context.Members.Where(m => m.ListId == listId).ToListAsync();

        var addedUserIds = members.Select(f => f.UserId).ToList();

        var membersToAdd = users.Where(u => !addedUserIds.Contains(u.Id)).ToList();


        ListMembersViewModel model = new()
        {
            Users = membersToAdd,
            Members = members,
            CurrentUser = CurrentUser,
        };
        return View(model);
    }
}
