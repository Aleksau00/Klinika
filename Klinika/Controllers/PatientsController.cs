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
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Secretary,Doctor,Administrator")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients([FromQuery] string? term = null)
        {
            var query = _context.Patients
                .Include(p => p.Guardian)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                var normalized = term.Trim().ToLower();
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(normalized) ||
                    p.LastName.ToLower().Contains(normalized) ||
                    p.JMBG.Contains(normalized) ||
                    (p.PhoneNumber ?? string.Empty).Contains(normalized) ||
                    (p.Email != null && p.Email.ToLower().Contains(normalized)));
            }

            var patients = await query
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Take(200)
                .ToListAsync();

            return Ok(patients.Select(MapDto));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Secretary,Doctor,Administrator")]
        public async Task<ActionResult<PatientDto>> GetPatientById(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Guardian)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound(new { message = $"Patient with ID {id} not found" });
            }

            return Ok(MapDto(patient));
        }

        [HttpPost]
        [Authorize(Roles = "Secretary,Administrator")]
        public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] CreatePatientRequest request)
        {
            var validation = await ValidateRequest(request, null);
            if (validation != null)
            {
                return validation;
            }

            var patient = new Patient
            {
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? string.Empty : request.PhoneNumber.Trim(),
                JMBG = request.JMBG,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                BloodType = request.BloodType,
                AddressId = request.AddressId,
                GuardianId = request.GuardianId,
                NoShowCount = 0,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var created = await _context.Patients
                .Include(p => p.Guardian)
                .FirstAsync(p => p.Id == patient.Id);

            return CreatedAtAction(nameof(GetPatientById), new { id = created.Id }, MapDto(created));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Secretary,Administrator")]
        public async Task<ActionResult<PatientDto>> UpdatePatient(int id, [FromBody] UpdatePatientRequest request)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
            if (patient == null)
            {
                return NotFound(new { message = $"Patient with ID {id} not found" });
            }

            var validation = await ValidateRequest(request, id);
            if (validation != null)
            {
                return validation;
            }

            patient.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            patient.FirstName = request.FirstName;
            patient.LastName = request.LastName;
            patient.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? string.Empty : request.PhoneNumber.Trim();
            patient.JMBG = request.JMBG;
            patient.Gender = request.Gender;
            patient.DateOfBirth = request.DateOfBirth;
            patient.BloodType = request.BloodType;
            patient.AddressId = request.AddressId;
            patient.GuardianId = request.GuardianId;

            await _context.SaveChangesAsync();

            var updated = await _context.Patients
                .Include(p => p.Guardian)
                .FirstAsync(p => p.Id == patient.Id);

            return Ok(MapDto(updated));
        }

        private async Task<ActionResult<PatientDto>?> ValidateRequest(UpdatePatientRequest request, int? currentPatientId)
        {
            var createLike = new CreatePatientRequest
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                JMBG = request.JMBG,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                BloodType = request.BloodType,
                AddressId = request.AddressId,
                GuardianId = request.GuardianId,
            };

            return await ValidateRequest(createLike, currentPatientId);
        }

        private async Task<ActionResult<PatientDto>?> ValidateRequest(CreatePatientRequest request, int? currentPatientId)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            {
                return BadRequest(new { message = "First name and last name are required." });
            }

            var normalizedEmail = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            var normalizedPhone = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();

            var age = DateTime.UtcNow.Year - request.DateOfBirth.Year;
            if (request.DateOfBirth.Date > DateTime.UtcNow.Date.AddYears(-age))
            {
                age -= 1;
            }

            var isMinor = age < 18;

            if (isMinor && !request.GuardianId.HasValue)
            {
                return BadRequest(new { message = "Patients under 18 must have a linked guardian." });
            }

            if (string.IsNullOrWhiteSpace(request.JMBG) || request.JMBG.Length != 13 || !request.JMBG.All(char.IsDigit))
            {
                return BadRequest(new { message = "JMBG must be exactly 13 digits." });
            }

            if (string.IsNullOrWhiteSpace(request.BloodType))
            {
                return BadRequest(new { message = "Blood type is required." });
            }

            if (!string.IsNullOrWhiteSpace(normalizedEmail))
            {
                var emailExists = await _context.Patients.AnyAsync(p => p.Email == normalizedEmail && (!currentPatientId.HasValue || p.Id != currentPatientId.Value));
                if (emailExists)
                {
                    return BadRequest(new { message = "Patient email already exists." });
                }
            }

            var jmbgExists = await _context.Patients.AnyAsync(p => p.JMBG == request.JMBG && (!currentPatientId.HasValue || p.Id != currentPatientId.Value));
            if (jmbgExists)
            {
                return BadRequest(new { message = "Patient JMBG already exists." });
            }

            if (request.GuardianId.HasValue)
            {
                var guardianExists = await _context.Guardians.AnyAsync(g => g.Id == request.GuardianId.Value);
                if (!guardianExists)
                {
                    return BadRequest(new { message = "Guardian not found." });
                }
            }

            return null;
        }

        private static PatientDto MapDto(Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                Email = patient.Email,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                PhoneNumber = patient.PhoneNumber ?? string.Empty,
                JMBG = patient.JMBG,
                Gender = patient.Gender,
                DateOfBirth = patient.DateOfBirth,
                BloodType = patient.BloodType,
                NoShowCount = patient.NoShowCount,
                AddressId = patient.AddressId,
                GuardianId = patient.GuardianId,
                GuardianName = patient.Guardian != null ? $"{patient.Guardian.FirstName} {patient.Guardian.LastName}" : null,
                CreatedAt = patient.CreatedAt,
            };
        }
    }
}