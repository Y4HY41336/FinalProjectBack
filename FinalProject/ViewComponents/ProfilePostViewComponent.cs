using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.HomeViewModels;
using FinalProject.ViewModels.ProfileViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewComponents
{
    public class ProfilePostViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;


        public ProfilePostViewComponent(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IViewComponentResult> InvokeAsync([FromQuery]string? TabId)
        {
            if (string.IsNullOrEmpty(TabId))
            {
                TabId = HttpContext.Session.GetString("ProfileTabId") ?? "Posts";
            }
            else
            {
                HttpContext.Session.SetString("ProfileTabId", TabId);
            }

            var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
            var postQuery = _context.Posts.AsQueryable();

            var bookmarks = await _context.Bookmarks.Where(b => b.UserId == user.Id).ToListAsync();
            var likes = await _context.Likes.Where(b => b.UserId == user.Id).ToListAsync();
            var reposts = await _context.Reposts.Where(b => b.UserId == user.Id).ToListAsync();
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
                                           .Take(30)
                                           .OrderByDescending(post => post.CreatedDate)
                                           .ToListAsync();
                    break;

                case "Replies":
                    posts = await postQuery.Where(p => p.IsActive)
                                           .Where(p => p.User.IsActive == true)
                                           .Where(p => p.UserId == user.Id)
                                           .Where(p => p.IsReply || p.IsQuote)
                                           .Include(p => p.User)
                                           .Take(30)
                                           .OrderByDescending(post => post.CreatedDate)
                                           .ToListAsync();
                    break;

                case "Likes":
                    var likesUserIds = likes.Select(f => f.PostId).ToList();

                    posts = await postQuery.Where(p => p.IsActive)
                                           .Where(p => likesUserIds.Contains(p.Id))
                                           .Where(p => p.User.IsActive == true)
                                           .Include(p => p.User)
                                           .Take(30)
                                           .OrderByDescending(post => post.CreatedDate)
                                           .ToListAsync();
                    break;

                case "Highlights":
                    posts = await postQuery.Where(p => p.IsActive)
                                           .Where(p => p.User.IsActive == true)
                                           .Where(p => !p.IsReply)
                                           .Where(p => p.UserId == user.Id)
                                           .Include(p => p.User)
                                           .Take(30)
                                           .OrderByDescending(post => post.CreatedDate)
                                           .ToListAsync();
                    break;

                default:
                    posts = await postQuery.Where(p => p.IsActive)
                                           .Where(p => p.User.IsActive == true)
                                           .Where(p => !p.IsReply)
                                           .Where(p => p.UserId == user.Id)
                                           .Include(p => p.User)
                                           .Take(30)
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
}
