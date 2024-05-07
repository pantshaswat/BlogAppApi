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
public class ReactionController : ControllerBase
{
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
        Console.WriteLine(id);

        if (id != reaction.ReactionId)
        {
            return BadRequest();
        }


        var user = await _context.Users.FindAsync(reaction.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var rec = await _context.Reactions.FindAsync(id);
        if (rec == null)
        {
            Console.WriteLine("Not found");
            return NotFound();
        }

        // Update the fetched entity directly
        rec.BlogId = reaction.BlogId;
        rec.CommentId = reaction.CommentId;
        rec.Type = reaction.Type;
        rec.UserId = reaction.UserId;

        try
        {
            Console.WriteLine(reaction);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Reaction updated" });
        }
        catch (DbUpdateConcurrencyException)
        {
            Console.WriteLine("Some Error occured");
            if (!ReactionExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReaction([FromRoute] int id)
    {
        var reaction = await _context.Reactions.FindAsync(id);
        if (reaction == null)
        {
            return NotFound();
        }

        _context.Reactions.Remove(reaction);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    private bool ReactionExists(int id)
    {
        return _context.Reactions.Any(e => e.ReactionId == id);
    }

};