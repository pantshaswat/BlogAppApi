using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blogAppApi.Models;
using blogAppApi.Data;
using Microsoft.EntityFrameworkCore;
namespace blogApp.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase{
        private readonly AppDbContext _context;
        public BlogController(AppDbContext context)
        {
            _context = context;
        }

      [HttpGet("paginate")]
public async Task<ActionResult<IEnumerable<Blog>>> GetBlogs(int pageNumber = 1, int pageSize = 10)
{
    var blogs = await _context.Blogs
        .Select(blog => new
        {
            blog.BlogId,
            blog.Title,
            blog.Body,
            blog.Images,
            blog.UserId,
            AuthorUsername = blog.Author.Username,
            blog.PostedDate,
            Comments = blog.Comments.Select(comment => new
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
            Reactions = blog.Reactions.Select(reaction => new
            {
                reaction.ReactionId,
                reaction.Type,
                ReactorUsername = reaction.Reactor.Username
            }).ToList()
        })
        .OrderByDescending(blog => blog.PostedDate)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return Ok(blogs);
}

      [HttpGet("popularity")]
public async Task<ActionResult<IEnumerable<Blog>>> GetPopularBlogs()
{
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

    // Calculate popularity for each blog
    var popularBlogs = blogs.Select(blog => new
    {
        Blog = blog,
        Popularity = CalculatePopularity(blog)
    })
    .OrderByDescending(blog => blog.Popularity)
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

    return Ok(popularBlogs);
}

[HttpGet("random")]
public async Task<ActionResult<IEnumerable<Blog>>> GetRandomBlogs()
{
    // Fetch all blogs
    var blogs = await _context.Blogs
    .Include(blog => blog.Author)
        .Include(blog => blog.Comments)
            .ThenInclude(comment => comment.Commenter)
        .Include(blog => blog.Comments)
            .ThenInclude(comment => comment.Reactions)
                .ThenInclude(reaction => reaction.Reactor)
        .Include(blog => blog.Reactions)
            .ThenInclude(reaction => reaction.Reactor)
        .Select(blog => new
        {
            Blog = blog,
            RandomOrder = Guid.NewGuid() // Assign a random Guid to each blog
        })
        .ToListAsync();

    // Shuffle the blogs
    var random = new Random();
    var shuffledBlogs = blogs.OrderBy(x => random.Next()).ToList();

    // Extract the blog details
    var result = shuffledBlogs.Select(x => new
    {
        x.Blog.BlogId,
        x.Blog.Title,
        x.Blog.Body,
        x.Blog.Images,
        x.Blog.UserId,
        AuthorUsername = x.Blog.Author.Username,
        x.Blog.PostedDate,
        Comments = x.Blog.Comments.Select(comment => new
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
        Reactions = x.Blog.Reactions.Select(reaction => new
        {
            reaction.ReactionId,
            reaction.Type,
            ReactorUsername = reaction.Reactor.Username
        }).ToList()
    });

    return Ok(result);
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


         [HttpGet("{id}")]
public async Task<ActionResult<Blog>> GetBlog([FromRoute] int id)
{
    var blog = await _context.Blogs
        .Where(b => b.BlogId == id)
        .Select(blog => new
        {
            blog.BlogId,
            blog.Title,
            blog.Body,
            blog.Images,
            blog.UserId, // Include the UserId property of Blog
            AuthorUsername = blog.Author.Username, // Include only the Username property of Author
            blog.PostedDate,
            Comments = blog.Comments.Select(comment => new
            {
                comment.CommentId,
                comment.Content,
                CommenterUsername = comment.Commenter.Username, // Include only the Username property of Commenter
                comment.CommentedDate,
                Reactions = comment.Reactions.Select(reaction => new
                {
                    reaction.ReactionId,
                    reaction.Type,
                    ReactorUsername = reaction.Reactor.Username // Include only the Username property of Reactor
                }).ToList()
            }).
            OrderByDescending(comment => comment.CommentedDate).
            ToList(),
            Reactions = blog.Reactions.Select(reaction => new
            {
                reaction.ReactionId,
                reaction.Type,
                ReactorUsername = reaction.Reactor.Username // Include only the Username property of Reactor
            }).ToList()
        })
        .ToListAsync();

    if (blog == null)
    {
        return NotFound();
    }

    return Ok(blog);
}
[HttpGet("User/{userId}")]
public async Task<ActionResult<Blog>> GetUsersBlog([FromRoute] int userId)
{
    var blog = await _context.Blogs
        .Where(b => b.UserId == userId)
        .Select(blog => new
        {
            blog.BlogId,
            blog.Title,
            blog.Body,
            blog.Images,
            blog.UserId, // Include the UserId property of Blog
            AuthorUsername = blog.Author.Username, // Include only the Username property of Author
            blog.PostedDate,
            Comments = blog.Comments.Select(comment => new
            {
                comment.CommentId,
                comment.Content,
                CommenterUsername = comment.Commenter.Username, // Include only the Username property of Commenter
                comment.CommentedDate,
                Reactions = comment.Reactions.Select(reaction => new
                {
                    reaction.ReactionId,
                    reaction.Type,
                    ReactorUsername = reaction.Reactor.Username // Include only the Username property of Reactor
                }).ToList()
            }).
            OrderByDescending(comment => comment.CommentedDate).
            ToList(),
            Reactions = blog.Reactions.Select(reaction => new
            {
                reaction.ReactionId,
                reaction.Type,
                ReactorUsername = reaction.Reactor.Username // Include only the Username property of Reactor
            }).ToList()
        })
        .ToListAsync();

    if (blog == null)
    {
        return NotFound();
    }

    return Ok(blog);
}


        [HttpPost]
        public async Task<ActionResult<Blog>> PostBlog(Blog blog)
        {
            blog.PostedDate = DateTime.Now;
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlog", new { id = blog.BlogId }, blog);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlog([FromRoute] int id, Blog blog)
        {
            if (id != blog.BlogId)
            {
                Console.WriteLine("BlogId does not match");
                return BadRequest();
            }
            // blog.PostedDate = DateTime.Now;
            _context.Entry(blog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(blog);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog([FromRoute] int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.BlogId == id);
        }
    }