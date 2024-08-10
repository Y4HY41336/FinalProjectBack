using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FinalProject.ViewComponents;

public class SearchContentViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public SearchContentViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] string? TabId, string? Search)
    {
        ViewBag.TabId = HttpContext.Session.GetString("SearchTabId");
        if (string.IsNullOrEmpty(TabId))
        {
            TabId = HttpContext.Session.GetString("SearchTabId");
        }
        else
        {
            HttpContext.Session.SetString("SearchTabId", TabId);
        }

        if (Search == null)
        {
            Search = HttpContext.Session.GetString("SearchTerm");
        }


        var searchTerm = Search.ToLower();

        var FilteredPost = new List<Post>();

        

        var filteredPost = await _context.Posts.Where(p => p.User.Name.ToLower().Contains(searchTerm) || p.User.UserName.ToLower().Contains(searchTerm) || p.Content.ToLower().Contains(searchTerm))
                                               .Where(p => p.IsActive)
                                               .Take(30)
                                               .Include(p => p.User)
                                               .ToListAsync();

        if (filteredPost == null)
        {
            filteredPost = new()
            {

            };
        }


        var filteredUser = await _context.Users.Where(p => p.Name.ToLower().Contains(searchTerm) || p.UserName.ToLower().Contains(searchTerm))
                                               .Where(p => p.IsActive)
                                               .Take(30)
                                               .ToListAsync();

        if (filteredUser == null)
        {
            filteredUser = new()
            {

            };
        }

        var filteredList = await _context.UserLists.Where(p => p.Name.ToLower().Contains(searchTerm) || p.Description.ToLower().Contains(searchTerm))
                                                   .Where(p => !p.IsDeleted && !p.IsPrivate)
                                                   .Take(30)
                                                   .ToListAsync();

        if (filteredList == null)
        {
            filteredList = new()
            {

            };
        }
        
        SearchViewModel SearchResult = new()
        {
            Posts = filteredPost,
            Users = filteredUser,
            Lists = filteredList,
            TabId = TabId,
        };

        return View(SearchResult);
    }
}
