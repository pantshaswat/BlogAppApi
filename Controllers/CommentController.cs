using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blogAppApi.Models;
using blogAppApi.Data;
using Microsoft.EntityFrameworkCore;
namespace blogApp.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase{
        private readonly AppDbContext _context;
        public CommentController(AppDbContext context)
        {
            _context = context;
        }

       [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
           var comments = await _context.Comments.ToListAsync();
           if (comments == null)
            {
                return NotFound();
            }
            return Ok(comments);
            }    
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment([FromRoute] int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            return Ok(comment);
        }   
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            comment.CommentedDate = DateTime.Now;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetComment", new { id = comment.CommentId }, comment);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment([FromRoute] int id, Comment comment)
        {
            if (id != comment.CommentId)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
            }
            return NoContent();
        }  
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }  
        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }       
            
}