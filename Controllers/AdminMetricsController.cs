using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blogAppApi.Models;
using blogAppApi.Data;
using Microsoft.EntityFrameworkCore;
namespace blogApp.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class AdminMetricsController : ControllerBase{
        private readonly AppDbContext _context;
        public AdminMetricsController(AppDbContext context)
        {
            _context = context;
        }
     

[HttpGet("monthlycount")]
public async Task<ActionResult<IEnumerable<object>>> GetMonthlyBlogPostCount()
{
    var monthlyCounts = await _context.Blogs
        .GroupBy(blog => new { blog.PostedDate.Year, blog.PostedDate.Month })
        .Select(group => new
        {
            Year = group.Key.Year,
            Month = group.Key.Month,
            TotalBlogPosts = group.Count(),
            TotalUpvotes = group.SelectMany(blog => blog.Reactions).Count(reaction => reaction.Type == "Upvote"),
            TotalDownvotes = group.SelectMany(blog => blog.Reactions).Count(reaction => reaction.Type == "Downvote"),
            TotalComments = group.SelectMany(blog => blog.Comments).Count()
        })
        .ToListAsync();

    return Ok(monthlyCounts);
}
        [HttpGet("counts")]
        public async Task<ActionResult<AdminMetrics>> GetAdminMetrics()
        {
            var adminMetrics = new AdminMetrics
            {
                Date = DateTime.Now,
                TotalBlogPosts = await _context.Blogs.CountAsync(),
                TotalUpvotes = await _context.Reactions.CountAsync(reaction => reaction.Type == "Upvote"),
                TotalDownvotes = await _context.Reactions.CountAsync(reaction => reaction.Type == "Downvote"),
                TotalComments = await _context.Comments.CountAsync()
            };

            return Ok(adminMetrics);
        }
      [HttpGet("top10")]
public async Task<ActionResult<IEnumerable<object>>> GetTop10PostsAndUsers()
{
    // Retrieve all blogs with related entities
    var blogs = await _context.Blogs
        .Include(blog => blog.Author)
        .Include(blog => blog.Comments)
            .ThenInclude(comment => comment.Commenter)
        .Include(blog => blog.Comments)
            .ThenInclude(comment => comment.Reactions)
                .ThenInclude(reaction => reaction.Reactor)
        .Include(blog => blog.Reactions)
            .ThenInclude(reaction => reaction.Reactor)
        .ToListAsync();

    // Calculate popularity for each blog and sum up popularity for each user
    var userPopularity = new Dictionary<int, double>(); // UserId to cumulative popularity score
    foreach (var blog in blogs)
    {
        var popularity = CalculatePopularity(blog);
        if (!userPopularity.ContainsKey(blog.UserId))
        {
            userPopularity[blog.UserId] = popularity;
        }
        else
        {
            userPopularity[blog.UserId] += popularity;
        }
    }

    // Retrieve top 10 users based on cumulative popularity scores
    var top10Users = userPopularity.OrderByDescending(kv => kv.Value)
                                   .Take(10)
                                   .ToList();

    // Retrieve top 10 blogs
    var popularBlogs = blogs.Select(blog => new
    {
        Blog = blog,
        Popularity = CalculatePopularity(blog)
    })
    .OrderByDescending(blog => blog.Popularity)
    .Take(10)
    .Select(blog => new
    {
        blog.Blog.BlogId,
        blog.Blog.Title,
        blog.Blog.Body,
        blog.Blog.Images,
        blog.Blog.UserId,
        AuthorUsername = blog.Blog.Author.Username,
        blog.Blog.PostedDate,
        Comments = blog.Blog.Comments.Select(comment => new
        {
            comment.CommentId,
            comment.Content,
            CommenterUsername = comment.Commenter.Username,
            comment.CommentedDate,
            Reactions = comment.Reactions.Select(reaction => new
            {
                reaction.ReactionId,
                reaction.Type,
                ReactorUsername = reaction.Reactor.Username
            }).ToList()
        }).ToList(),
        Reactions = blog.Blog.Reactions.Select(reaction => new
        {
            reaction.ReactionId,
            reaction.Type,
            ReactorUsername = reaction.Reactor.Username
        }).ToList()
    })
    .ToList();

    return Ok(new { Top10Blogs = popularBlogs, Top10Users = top10Users });
}



// Method to calculate popularity for a single blog
private double CalculatePopularity(Blog blog)
{
    // You need to define the weightage for upvotes, downvotes, and comments
    double upvoteWeightage = 2; // Example weightage for upvotes
    double downvoteWeightage = -1; // Example weightage for downvotes
    double commentWeightage = 1; // Example weightage for comments

    // Count the number of upvotes, downvotes, and comments for the blog
    int upvotes = blog.Reactions.Count(r => r.Type == "Upvote");
    int downvotes = blog.Reactions.Count(r => r.Type == "Downvote");
    int comments = blog.Comments.Count;

    // Calculate blog popularity using the provided formula
    double popularity = (upvoteWeightage * upvotes) + (downvoteWeightage * downvotes) + (commentWeightage * comments);

    return popularity;
}

   
       
    }