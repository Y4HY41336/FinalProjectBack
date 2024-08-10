using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewComponents;

public class CustomUserPostViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public CustomUserPostViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] string? TabId, [FromQuery] string? userId)
    {
        if (string.IsNullOrEmpty(TabId))
        {
            TabId = HttpContext.Session.GetString("CustomUserTabId") ?? "Posts";
        }
        else
        {
            HttpContext.Session.SetString("CustomUserTabId", TabId);
        }

        if (userId == null)
        {
            userId = HttpContext.Session.GetString("CustomUserId");
        }
        var user = await _userManager.FindByIdAsync(userId);
        var CurrentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
        var postQuery = _context.Posts.AsQueryable();

        var bookmarks = await _context.Bookmarks.Where(b => b.UserId == CurrentUser.Id).ToListAsync();
        var likes = await _context.Likes.Where(b => b.UserId == CurrentUser.Id).ToListAsync();
        var reposts = await _context.Reposts.Where(b => b.UserId == CurrentUser.Id).ToListAsync();
        var postImages = await _context.PostImages.ToListAsync();

        List<Post> posts = null;

        switch (TabId)
        {
            case "Posts":
                posts = await postQuery.Where(p => p.IsActive)
                                       .Where(p => p.User.IsActive == true)
                                       .Where(p => !p.IsReply)
                                       .Where(p => p.UserId == user.Id)
                                       .Include(p => p.User)
                                       .OrderByDescending(post => post.CreatedDate)
                                       .ToListAsync();
                break;

            case "Replies":
                posts = await postQuery.Where(p => p.IsActive)
                                       .Where(p => p.User.IsActive == true)
                                       .Where(p => p.UserId == user.Id)
                                       .Where(p => p.IsReply || p.IsQuote)
                                       .Include(p => p.User)
                                       .OrderByDescending(post => post.CreatedDate)
                                       .ToListAsync();
                break;

            case "Highlights":
                posts = await postQuery.Where(p => p.IsActive)
                                       .Where(p => p.User.IsActive == true)
                                       .Where(p => !p.IsReply)
                                       .Where(p => p.UserId == user.Id)
                                       .Include(p => p.User)
                                       .OrderByDescending(post => post.CreatedDate)
                                       .ToListAsync();
                break;

            case "Subs":
                posts = await postQuery.Where(p => p.IsActive)
                                       .Where(p => p.User.IsActive == true)
                                       .Where(p => !p.IsReply)
                                       .Where(p => p.UserId == user.Id)
                                       .Include(p => p.User)
                                       .OrderByDescending(post => post.CreatedDate)
                                       .ToListAsync();
                break;

            case "Media":
                posts = await postQuery.Where(p => p.IsActive)
                                       .Where(p => p.User.IsActive == true)
                                       .Where(p => !p.IsReply)
                                       .Where(p => p.UserId == user.Id)
                                       .Include(p => p.User)
                                       .OrderByDescending(post => post.CreatedDate)
                                       .ToListAsync();
                break;

            default:
                posts = await postQuery.Where(p => p.IsActive)
                                       .Where(p => p.User.IsActive == true)
                                       .Where(p => !p.IsReply)
                                       .Where(p => p.UserId == user.Id)
                                       .Include(p => p.User)
                                       .OrderByDescending(post => post.CreatedDate)
                                       .ToListAsync();

                break;
        }


        PostViewModel model = new()
        {
            CurrentUser = CurrentUser,
            Posts = posts,
            User = user,
            Bookmarks = bookmarks,
            Likes = likes,
            Reposts = reposts,
            PostImages = postImages,

        };



        return View(model);
    }

}
