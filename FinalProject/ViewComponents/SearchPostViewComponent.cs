using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.HomeViewModels;
using FinalProject.ViewModels.LayoutViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;

namespace FinalProject.ViewComponents;

public class SearchPostViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public SearchPostViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] string? TabId, List<Post>? Posts)
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
        var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

        var postQuery = _context.Posts.Where(p => p.IsActive).AsQueryable();
        var bookmarks = await _context.Bookmarks.Where(b => b.UserId == user.Id).ToListAsync();
        var likes = await _context.Likes.Where(b => b.UserId == user.Id).ToListAsync();
        var reposts = await _context.Reposts.Where(b => b.UserId == user.Id).ToListAsync();
        var postImages = await _context.PostImages.ToListAsync();
        var filteredPost = new List<Post>();

        var follows = await _context.Follows.Where(f => f.FollowerId == CurrentUser.Id).ToListAsync();

        switch (TabId)
        {
            case "Top":

                filteredPost = Posts.ToList();

                break;


            case "Latest":

                filteredPost = Posts.OrderByDescending(Post => Post.CreatedDate).ToList();

                break;


            case "People":



                break;

            case "Lists":



                break;

            default:



                break;
        }

        if (filteredPost == null)
        {
            filteredPost = new()
            {

            };
        }

        PostViewModel model = new()
        {
            Posts = filteredPost,
            User = user,
            Bookmarks = bookmarks,
            Likes = likes,
            Reposts = reposts,
            PostImages = postImages,
        };

        return View(model);
    }
}
