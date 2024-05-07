using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blogAppApi.Models;
using blogAppApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
namespace blogApp.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class ReactionController : ControllerBase{
        private readonly AppDbContext _context;
        public ReactionController(AppDbContext context)
        {
            _context = context;
        }
  

         [HttpGet]
        public async Task<ActionResult<IEnumerable<Reaction>>> GetReactions()
        {
           var reactions = await _context.Reactions.ToListAsync();
           return Ok(reactions);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Reaction>> GetReaction([FromRoute] int id)
        {
            var reaction = await _context.Reactions.FindAsync(id);
            return Ok(reaction);
        }
        [HttpPost]
        public async Task<ActionResult<Reaction>> PostBlogReaction(Reaction reaction)
        {   
            var reactor = await _context.Users.FindAsync(reaction.UserId);
            if (reaction.BlogId != null)
            {
                var reactedTo = await _context.Reactions.
                Include(r => r.Blog).   
                ThenInclude(b => b.Author).FirstOrDefaultAsync();
                var reactedToUserId = reactedTo.Blog.Author.UserId;
                if (reactedToUserId != reaction.UserId)
                {
                Notification Notification = new Notification
                {
                    Content = $"{reactor.Username} reacted to your blog",
                    NotificationDate = DateTime.Now,
                    UserId = reactedToUserId
                };
                _context.Notifications.Add(Notification);
                }
            }
            if (reaction.CommentId != null)
            {
               var reactedTo = await _context.Reactions.
                Include(r => r.Comment).   
                ThenInclude(b => b.Commenter).FirstOrDefaultAsync();
                var reactedToUserId = reactedTo.Comment.Commenter.UserId;
                if (reactedToUserId != reaction.UserId)
                {
                Notification Notification = new Notification
                {
                    Content = $"{reactor.Username} reacted to your comment",
                    NotificationDate = DateTime.Now,
                    UserId = reactedToUserId
                };
                _context.Notifications.Add(Notification);

                }
            }
          
            if (reactor == null)
            {
                return NotFound();
            }

           
            _context.Reactions.Add(reaction);
            await _context.SaveChangesAsync();
           

            
            return CreatedAtAction("GetReaction", new { id = reaction.ReactionId }, reaction);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReaction([FromRoute] int id, Reaction reaction)
        {
            if (id != reaction.ReactionId)
            {
                return BadRequest();
            }

            _context.Entry(reaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(reaction);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReactionExists(id))
                {
                    return NotFound();
                }
            }

            return NoContent();
        }

        private bool ReactionExists(int id)
        {
            return _context.Reactions.Any(e => e.ReactionId == id);
        }

            
}