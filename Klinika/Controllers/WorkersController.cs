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

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentWorker()
        {
            var workerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var worker = await _workerService.GetByIdAsync(workerId);

            return Ok(new
            {
                worker.Id,
                worker.Email,
                worker.FirstName,
                worker.LastName,
                Role = GetRoleName(worker) // Derive role from type
            });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")] // Only admins can create staff accounts
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
                return Ok(new
                {
                    worker.Id,
                    worker.Email,
                    worker.FirstName,
                    worker.LastName,
                    worker.PhoneNumber,
                    Role = GetRoleName(worker),
                    worker.IsActive
                });
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
                // Update worker logic here
                return Ok(new { Message = "Worker assigned to clinic successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
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

    // DTO
    public class AssignClinicRequest
    {
        public int? ClinicId { get; set; }
    }
}