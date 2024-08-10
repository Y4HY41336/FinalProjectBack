using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.Explore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;

namespace FinalProject.ViewComponents;

public class ExploreTrendViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public ExploreTrendViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] string? TabId)
    {
        var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

        var postQuery = _context.Trends.AsQueryable();

        List<Trend> trends = null;

        switch (TabId)
        {
            case "ForYou":
                

                trends = await postQuery.Include(p => p.PostTrends).Take(5).ToListAsync();

                break;

            case "Trending":
                trends = await postQuery.Include(p => p.PostTrends).ToListAsync();
                
                break;

            case "News":
                trends = await postQuery.Include(p => p.PostTrends).ToListAsync();

                break;

            case "Sports":
                trends = await postQuery.Include(p => p.PostTrends).ToListAsync();

                break;

            case "Entertainment":
                trends = await postQuery.Include(p => p.PostTrends).ToListAsync();

                break;

            default:
                trends = await postQuery.Include(p => p.PostTrends).Take(5).ToListAsync();

                break;
        }

        ExploreTrendsViewModel model = new()
        {
            Trends = trends,
            TabId = TabId,
        };
        

        return View(model);
    }
}
