using Klinika.Models;
using Klinika.Models.DTOs;
using Klinika.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Klinika.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkersController : ControllerBase
    {
        private readonly IWorkerService _workerService;

        public WorkersController(IWorkerService workerService)
        {
            _workerService = workerService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<WorkerDto>>> GetAllWorkers()
        {
            var workers = await _workerService.GetAllAsync();
            return Ok(workers.Select(MapWorkerDto));
        }

        [HttpGet("doctors")]
        [Authorize(Roles = "Secretary,Administrator")]
        public async Task<ActionResult<IEnumerable<WorkerDto>>> GetDoctors([FromQuery] int? clinicId = null)
        {
            var workers = await _workerService.GetAllAsync();

            var doctors = workers
                .Where(w => w is Doctor)
                .Where(w => w.IsActive)
                .Where(w => !clinicId.HasValue || w.ClinicId == clinicId.Value)
                .Select(MapWorkerDto)
                .ToList();

            return Ok(doctors);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentWorker()
        {
            var workerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(workerIdValue, out var workerId))
            {
                return Unauthorized(new { message = "Worker identifier is missing from the token." });
            }

            var worker = await _workerService.GetByIdAsync(workerId);
            return Ok(MapWorkerDto(worker));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateWorker([FromBody] CreateWorkerRequest request)
        {
            try
            {
                var worker = await _workerService.CreateAsync(request);
                return Ok(new { message = "Worker created successfully", workerId = worker.Id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetWorker(int id)
        {
            try
            {
                var worker = await _workerService.GetByIdAsync(id);
                return Ok(MapWorkerDto(worker));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateWorker(int id, [FromBody] UpdateWorkerRequest request)
        {
            try
            {
                var worker = await _workerService.UpdateAsync(id, request);
                return Ok(MapWorkerDto(worker));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/clinic")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> AssignWorkerToClinic(int id, [FromBody] AssignClinicRequest request)
        {
            try
            {
                var worker = await _workerService.GetByIdAsync(id);
                worker.ClinicId = request.ClinicId;
                return Ok(new { Message = "Worker assigned to clinic successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch("{id}/active")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SetWorkerActive(int id, [FromBody] SetActiveRequest request)
        {
            try
            {
                await _workerService.SetActiveAsync(id, request.IsActive);
                return Ok(new { message = request.IsActive ? "Worker activated successfully" : "Worker deactivated successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        private WorkerDto MapWorkerDto(Worker worker)
        {
            return new WorkerDto
            {
                Id = worker.Id,
                FirstName = worker.FirstName,
                LastName = worker.LastName,
                Email = worker.Email,
                PhoneNumber = worker.PhoneNumber,
                JMBG = worker.JMBG,
                Gender = worker.Gender,
                DateOfBirth = worker.DateOfBirth,
                CreatedAt = worker.CreatedAt,
                AddressId = worker.AddressId,
                Role = GetRoleName(worker),
                ClinicId = worker.ClinicId,
                ClinicName = worker.Clinic?.Name,
                IsActive = worker.IsActive,
                Specialty = worker is Doctor doctor ? doctor.Specialty : null,
                LicenseNumber = worker is Doctor doctorWorker ? doctorWorker.LicenseNumber : null,
                Qualification = worker is Secretary secretary ? secretary.Qualification : null,
                SeniorityLevel = worker is Administrator administrator ? administrator.SeniorityLevel : null,
            };
        }

        private string GetRoleName(Worker worker)
        {
            return worker switch
            {
                Administrator => "Administrator",
                Doctor => "Doctor",
                Secretary => "Secretary",
                _ => "Unknown"
            };
        }
    }

    public class AssignClinicRequest
    {
        public int? ClinicId { get; set; }
    }

    public class SetActiveRequest
    {
        public bool IsActive { get; set; }
    }
}