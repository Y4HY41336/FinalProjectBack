using FinalProject.Context;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FinalProject.Controllers;

public class BookmarksController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly NotificationService _notificationService;

    public BookmarksController(AppDbContext context, UserManager<AppUser> userManager, NotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    [Authorize]
    public IActionResult Index()
    {
        return View();
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
        return RedirectToAction("Index");
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

        return ViewComponent("Bookmark");
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

        return ViewComponent("Bookmark");
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

        return ViewComponent("Bookmark");

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

        return ViewComponent("Bookmark");
    }

    [Authorize]
    public async Task<IActionResult> ClearAllBookmarks(int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var bookmarks = await _context.Bookmarks.Where(b => b.UserId == user.Id).ToListAsync();

        foreach (var bookmark in bookmarks)
        {
            _context.Remove(bookmark);
        }

        _context.SaveChanges();

        return RedirectToAction("Index");
    }

}
