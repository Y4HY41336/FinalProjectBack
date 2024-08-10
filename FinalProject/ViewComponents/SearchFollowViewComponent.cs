using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.LayoutViewModels;
using FinalProject.ViewModels.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FinalProject.ViewComponents;

public class SearchFollowViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public SearchFollowViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] string? TabId, List<AppUser>? Users)
    {
        if (string.IsNullOrEmpty(TabId))
        {
            TabId = HttpContext.Session.GetString("SearchTabId") ?? "Top";
        }
        else
        {
            HttpContext.Session.SetString("SearchTabId", TabId);
        }
        var CurrentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

        var filteredUser = new List<AppUser>();

        switch (TabId)
        {
            case "Top":

                filteredUser = Users.Take(3).ToList();

                break;


            case "Latest":

                

                break;


            case "People":

                filteredUser = Users.ToList();

                break;

            case "Lists":



                break;

            default:
                


                break;
        }

        if (filteredUser == null)
        {
            filteredUser = new()
            {

            };
        }

        var follows = await _context.Follows.Where(f => f.FollowerId == CurrentUser.Id).ToListAsync();

        FollowUserViewModel newmodel = new()
        {
            Users = filteredUser,
            Follows = follows,
            CurrentUser = CurrentUser,
        };

        return View(newmodel);
    }
}

