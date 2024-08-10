using FinalProject.Context;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModels.HomeViewModels;
using FinalProject.ViewModels.Searc;
using FinalProject.ViewModels.Search;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FinalProject.Controllers;

public class SearchController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly NotificationService _notificationService;

    public SearchController(AppDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment, NotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
        _notificationService = notificationService;
    }
    [Authorize]
    public async Task<IActionResult> Index(string? Search)
    {

        ViewBag.TabId = HttpContext.Session.GetString("SearchTabId") ?? "Top";


        var searchTerm = Search.ToLower();

        HttpContext.Session.SetString("SearchTerm", searchTerm);



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
        };

        if (TempData["Redirected"] == null)
        {
            TempData["Redirected"] = true;

            return RedirectToAction("Index", new { Search = searchTerm});
        }

        TempData["Redirected"] = null;


        return View(SearchResult);

    }
    [ActionName("Index")]
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> IndexOf(string? Search, SearchViewModel? model)
    {
        if (model.Search == null)
        {
            model.Search = string.Empty;
        }

        ViewBag.TabId = HttpContext.Session.GetString("SearchTabId") ?? "Top";

        
        var searchTerm = model.Search.ToLower();
        if (Search != null)
        {
            searchTerm = Search.ToLower();
        }
        HttpContext.Session.SetString("SearchTerm", searchTerm);

        

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
        };


        return View(SearchResult);

    }
    
    [Authorize]
    public async Task<IActionResult> FollowList(int listId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        var list = await _context.UserLists.FirstOrDefaultAsync(p => p.Id == listId);
        if (list == null)
        {
            return NotFound();
        }

        var listFollower = await _context.ListFollowers.FirstOrDefaultAsync(p => p.ListId == listId && p.UserId == user.Id);
        if (listFollower == null)
        {
            ListFollower newListFollower = new()
            {
                ListId = listId,
                UserId = user.Id,
            };
            list.FollowerCount++;
            await _context.ListFollowers.AddAsync(newListFollower);
        }
        else
        {
            list.FollowerCount--;
            if (list.FollowerCount <= 0)
            {
                list.FollowerCount = 0;
            }
            _context.Remove(listFollower);
        }


        await _context.SaveChangesAsync();

        return ViewComponent("SearchList");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePost(CreatePostViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        Post newPost = new()
        {
            Content = model.Content,
            Image = model.Image,
            CreatedDate = DateTime.UtcNow,
            UserId = user.Id,
            IsActive = true,
        };
        await _context.Posts.AddAsync(newPost);
        await _context.SaveChangesAsync();


        model.PostId = newPost.Id;
        return RedirectToAction("CreateTrend", new { PostId = newPost.Id, TrendName = model.Content });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ReplyPost(CreatePostViewModel model, int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        var post = await _context.Posts.Where(p => p.IsActive).FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
        {
            return NotFound();
        }
        Post newPost = new()
        {
            Content = model.Content,
            Image = model.Image,
            CreatedDate = DateTime.UtcNow,
            UserId = user.Id,
            IsActive = true,
            CommentedPostId = post.Id,
            IsReply = true,
        };
        post.ReplyCount++;
        await _context.Posts.AddAsync(newPost);
        await _context.SaveChangesAsync();
        if (user.Id != post.UserId)
        {
            await _notificationService.SendNotificationAsync(user.Id, post.UserId, postId, "Reply", newPost.Content);
        }

        model.PostId = newPost.Id;
        return RedirectToAction("CreateTrend", new { PostId = newPost.Id, TrendName = model.Content });
    }

    [Authorize]
    public async Task<IActionResult> CreateTrend(CreatePostViewModel model)
    {
        string pattern = @"#\w+";

        var trendNames = Regex.Matches(model.TrendName, pattern);

        foreach (var name in trendNames)
        {
            var trend = await _context.Trends.FirstOrDefaultAsync(t => t.TrendName == name.ToString());

            if (trend == null)
            {
                Trend newTrend = new()
                {
                    TrendName = name.ToString(),
                    CreatedDate = DateTime.Now,
                };
                newTrend.PostCount++;
                await _context.Trends.AddAsync(newTrend);
                await _context.SaveChangesAsync();

                PostTrend newPostTrend = new()
                {
                    PostId = model.PostId,
                    TrendId = newTrend.Id,
                };

                await _context.PostTrends.AddAsync(newPostTrend);
                await _context.SaveChangesAsync();

            }
            else
            {
                trend.PostCount++;
                PostTrend newPostTrend = new()
                {
                    PostId = model.PostId,
                    TrendId = trend.Id,
                };

                await _context.PostTrends.AddAsync(newPostTrend);
            }
        }        

        await _context.SaveChangesAsync();

        var Search = HttpContext.Session.GetString("SearchTerm");

        return RedirectToAction("Index", new {Search = Search});
    }

    [Authorize]
    public async Task<IActionResult> DeletePost(int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        var post = await _context.Posts.Where(p => p.IsActive).FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
        {
            return NotFound();
        }
        var postTrends = await _context.PostTrends.Where(p => p.PostId == postId).ToListAsync();
        if (postTrends != null)
        {
            foreach (var postTrend in postTrends)
            {
                var trend = await _context.Trends.FirstOrDefaultAsync(p => p.Id == postTrend.TrendId);
                trend.PostCount--;
            }


        }

        post.IsActive = false;
        await _context.SaveChangesAsync();

        return ViewComponent("SearchContent");
    }

    [Authorize]
    public async Task<IActionResult> Follow(string userId)
    {
        var CurrentUser = await _userManager.GetUserAsync(User);
        var Followed = await _userManager.FindByIdAsync(userId);

        var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == CurrentUser.Id && f.FollowedId == Followed.Id);

        if (follow == null)
        {
            Follow newFollow = new()
            {
                FollowedId = Followed.Id,
                FollowerId = CurrentUser.Id,
                CreatedDate = DateTime.Now,
            };
            Followed.FollowerCount++;
            CurrentUser.FollowingCount++;
            await _context.Follows.AddAsync(newFollow);
            await _context.SaveChangesAsync();
        }
        else
        {
            Followed.FollowerCount--;
            CurrentUser.FollowingCount--;

            if (Followed.FollowerCount <= 0)
            {
                Followed.FollowerCount = 0;
            }
            if (CurrentUser.FollowingCount <= 0)
            {
                CurrentUser.FollowingCount = 0;
            }

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [Authorize]
    public async Task<IActionResult> Repost(int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var post = await _context.Posts.Where(p => p.IsActive).FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
        {
            return NotFound();
        }

        var repost = await _context.Reposts.FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == user.Id);
        if (repost == null)
        {
            Repost newRepost = new()
            {
                PostId = postId,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
            };
            post.RepostCount++;
            await _context.Reposts.AddAsync(newRepost);
            if (user.Id != post.UserId)
            {
                await _notificationService.SendNotificationAsync(user.Id, post.UserId, postId, "Repost", $"{user.Name} reposted your post");
            }
        }
        else
        {
            post.RepostCount--;
            _context.Remove(repost);
        }

        await _context.SaveChangesAsync();

        return ViewComponent("SearchContent");
    }

    [Authorize]
    public async Task<IActionResult> Like(int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var post = await _context.Posts.Where(p => p.IsActive).FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
        {
            return NotFound();
        }

        var like = await _context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == user.Id);
        if (like == null)
        {
            Like newlike = new()
            {
                PostId = postId,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
            };
            post.LikeCount++;
            await _context.Likes.AddAsync(newlike);
            if (user.Id != post.UserId)
            {
                await _notificationService.SendNotificationAsync(user.Id, post.UserId, postId, "Like", $"{user.Name} liked your post");
            }
        }
        else
        {
            post.LikeCount--;
            _context.Remove(like);
        }

        await _context.SaveChangesAsync();

        return ViewComponent("SearchContent");
    }

    [Authorize]
    public async Task<IActionResult> Bookmark(int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var post = await _context.Posts.Where(p => p.IsActive).FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
        {
            return NotFound();
        }

        var bookMark = await _context.Bookmarks.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == user.Id);
        if (bookMark == null)
        {
            Bookmark newBookmark = new()
            {
                PostId = postId,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
            };
            post.BookmarkCount++;
            await _context.Bookmarks.AddAsync(newBookmark);

        }
        else
        {
            post.BookmarkCount--;
            _context.Remove(bookMark);
        }

        await _context.SaveChangesAsync();

        return ViewComponent("SearchContent");
    }

    [Authorize]
    public async Task<IActionResult> Filter(string? TabId)
    {
        var Search = HttpContext.Session.GetString("SearchTerm");
        HttpContext.Session.SetString("SearchTabId", TabId);
        return ViewComponent("SearchContent", new { TabId = TabId, Search = Search});
    }

}
