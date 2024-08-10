using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FinalProject.ViewComponents;

public class PostViewComponent: ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public PostViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] string? TabId)
    {

        if (string.IsNullOrEmpty(TabId))
        {
            TabId = HttpContext.Session.GetString("HomeTabId") ?? "ForYou";
        }
        else
        {
            HttpContext.Session.SetString("HomeTabId", TabId);
        }


        var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

        var postQuery = _context.Posts.Where(p=> p.IsActive).AsQueryable();
        var bookmarks = await _context.Bookmarks.Where(b => b.UserId == user.Id).ToListAsync();
        var likes = await _context.Likes.Where(b => b.UserId == user.Id).ToListAsync();
        var reposts = await _context.Reposts.Where(b => b.UserId == user.Id).ToListAsync();
        var postImages = await _context.PostImages.ToListAsync();


        List<Post> posts = null;

        switch (TabId)
        {
            case "ForYou":
                posts = await _context.Posts.Where(p => p.IsActive)
                                        .Where(p => p.User.IsActive == true)
                                        .Where(p => !p.IsReply)
                                        .Include(p => p.User)   
                                        .Take(50)
                                        .OrderByDescending(post => post.CreatedDate)
                                        .ToListAsync();
                break;

            case "Following":
                var follows = await _context.Follows.Where(f => f.FollowerId == user.Id).ToListAsync();
                var followedUserIds = follows.Select(f => f.FollowedId).ToList();

                posts = await _context.Posts.Where(p => p.User.IsActive == true)
                                                .Where(p => p.IsActive == true)
                                                .Where(p => !p.IsReply)
                                                .Where(p => p.IsActive && followedUserIds.Contains(p.UserId) || p.UserId == user.Id)
                                                .Include(p => p.User)
                                                .Take(50)
                                                .OrderByDescending(p => p.CreatedDate)
                                                .ToListAsync();
                break;

            default:
                posts = await _context.Posts.Where(p => p.IsActive)
                                        .Where(p => p.User.IsActive == true)
                                        .Where(p => !p.IsReply)
                                        .Include(p => p.User)
                                        .Take(50)
                                        .OrderByDescending(post => post.CreatedDate)
                                        .ToListAsync();

                break;
        }


        PostViewModel model = new()
        {
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
