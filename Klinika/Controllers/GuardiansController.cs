using Klinika.DATA;
using Klinika.Models;
using Klinika.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Klinika.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GuardiansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GuardiansController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Secretary,Administrator")]
        public async Task<ActionResult<IEnumerable<GuardianDto>>> GetGuardians([FromQuery] string? term = null)
        {
            var query = _context.Guardians.AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                var normalized = term.Trim().ToLower();
                query = query.Where(g =>
                    g.FirstName.ToLower().Contains(normalized) ||
                    g.LastName.ToLower().Contains(normalized) ||
                    g.JMBG.Contains(normalized) ||
                    g.PhoneNumber.Contains(normalized) ||
                    (g.Email != null && g.Email.ToLower().Contains(normalized))
                );
            }

            var guardians = await query
                .OrderBy(g => g.LastName)
                .ThenBy(g => g.FirstName)
                .Take(200)
                .ToListAsync();

            return Ok(guardians.Select(MapDto));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Secretary,Administrator")]
        public async Task<ActionResult<GuardianDto>> GetGuardianById(int id)
        {
            var guardian = await _context.Guardians.FirstOrDefaultAsync(g => g.Id == id);
            if (guardian == null)
            {
                return NotFound(new { message = $"Guardian with ID {id} not found" });
            }

            return Ok(MapDto(guardian));
        }

        [HttpPost]
        [Authorize(Roles = "Secretary,Administrator")]
        public async Task<ActionResult<GuardianDto>> CreateGuardian([FromBody] CreateGuardianRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            {
                return BadRequest(new { message = "First name and last name are required." });
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return BadRequest(new { message = "Phone number is required." });
            }

            if (string.IsNullOrWhiteSpace(request.JMBG) || request.JMBG.Length != 13 || !request.JMBG.All(char.IsDigit))
            {
                return BadRequest(new { message = "JMBG must be exactly 13 digits." });
            }

            var age = DateTime.UtcNow.Year - request.DateOfBirth.Year;
            if (request.DateOfBirth.Date > DateTime.UtcNow.Date.AddYears(-age))
            {
                age -= 1;
            }

            if (age < 18)
            {
                return BadRequest(new { message = "Guardian must be at least 18 years old." });
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailExists = await _context.Set<Person>().AnyAsync(p => p.Email == request.Email);
                if (emailExists)
                {
                    return BadRequest(new { message = "Email already exists." });
                }
            }

            var jmbgExists = await _context.Set<Person>().AnyAsync(p => p.JMBG == request.JMBG);
            if (jmbgExists)
            {
                return BadRequest(new { message = "JMBG already exists." });
            }

            var guardian = new Guardian
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                JMBG = request.JMBG,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                AddressId = request.AddressId,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Guardians.Add(guardian);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGuardianById), new { id = guardian.Id }, MapDto(guardian));
        }

        private static GuardianDto MapDto(Guardian guardian)
        {
            return new GuardianDto
            {
                Id = guardian.Id,
                Email = guardian.Email,
                FirstName = guardian.FirstName,
                LastName = guardian.LastName,
                PhoneNumber = guardian.PhoneNumber,
                JMBG = guardian.JMBG,
                Gender = guardian.Gender,
                DateOfBirth = guardian.DateOfBirth,
                AddressId = guardian.AddressId,
                CreatedAt = guardian.CreatedAt,
            };
        }
    }
}
