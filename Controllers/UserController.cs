using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blogAppApi.Models;
using blogAppApi.Data;
using Microsoft.EntityFrameworkCore;
namespace blogApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    public UserController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser([FromRoute] int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // PUT: api/User/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser([FromRoute] int id, User user)
    {
        if (id != user.UserId)
        {
            return BadRequest();
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
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

    // POST: api/User
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        if (UserEmailExists(user.Email))
        {
            return Conflict("Email is already taken");
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUser", new { id = user.UserId }, user);
    }

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.UserId == id);
    }
    private bool UserEmailExists(string email)
    {
        return _context.Users.Any(e => e.Email == email);
    }

    //POST: api/User/Login
    [HttpPost("Login")]
    public async Task<ActionResult<User>> Login(string Email, string Password)
    {
        var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
        if (userInDb == null)
        {
            return NotFound("Invalid email");
        }
        if (userInDb.Password != Password)
        {
            return BadRequest("Invalid password");
        }
        return Ok(userInDb);
    }
}