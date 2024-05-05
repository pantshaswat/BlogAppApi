using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blogAppApi.Models;
using blogAppApi.Data;
using Microsoft.EntityFrameworkCore;
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