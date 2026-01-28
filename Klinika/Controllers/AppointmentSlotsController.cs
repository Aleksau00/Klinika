using Klinika.Models.DTOs;
using Klinika.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Klinika.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentSlotsController : ControllerBase
    {
        private readonly IAppointmentSlotService _slotService;

        public AppointmentSlotsController(IAppointmentSlotService slotService)
        {
            _slotService = slotService;
        }

        // GET: api/appointmentslots/doctor/5
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<AppointmentSlotResponse>>> GetDoctorSlots(
            int doctorId,
            [FromQuery] DateOnly? fromDate = null,
            [FromQuery] DateOnly? toDate = null)
        {
            var slots = await _slotService.GetDoctorSlotsAsync(doctorId, fromDate, toDate);
            return Ok(slots);
        }

        // GET: api/appointmentslots/doctor/5/available
        [HttpGet("doctor/{doctorId}/available")]
        public async Task<ActionResult<IEnumerable<AppointmentSlotResponse>>> GetAvailableSlots(
            int doctorId,
            [FromQuery] DateOnly? fromDate = null,
            [FromQuery] DateOnly? toDate = null)
        {
            var slots = await _slotService.GetAvailableSlotsAsync(doctorId, fromDate, toDate);
            return Ok(slots);
        }

        // GET: api/appointmentslots/my-slots
        [HttpGet("my-slots")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<IEnumerable<AppointmentSlotResponse>>> GetMySlots(
            [FromQuery] DateOnly? fromDate = null,
            [FromQuery] DateOnly? toDate = null)
        {
            var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var slots = await _slotService.GetDoctorSlotsAsync(doctorId, fromDate, toDate);
            return Ok(slots);
        }

        // POST: api/appointmentslots
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<AppointmentSlotResponse>> CreateSlot(
            [FromQuery] DateOnly date,
            [FromQuery] TimeOnly startTime)
        {
            try
            {
                var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var slot = await _slotService.CreateSlotAsync(doctorId, date, startTime);
                return CreatedAtAction(nameof(GetDoctorSlots), new { doctorId = slot.DoctorId }, slot);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // POST: api/appointmentslots/weekly
        [HttpPost("weekly")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<IEnumerable<AppointmentSlotResponse>>> CreateWeeklySlots()
        {
            try
            {
                var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var request = new CreateWeeklySlotsRequest { DoctorId = doctorId };
                var slots = await _slotService.CreateWeeklySlotsAsync(request);
                return Ok(new { Message = $"Created {slots.Count()} slots for the next 7 days", Slots = slots });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/appointmentslots/custom
        [HttpPost("custom")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<IEnumerable<AppointmentSlotResponse>>> CreateCustomSlots(
            [FromBody] CreateCustomSlotsRequest request)
        {
            try
            {
                var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var slots = await _slotService.CreateCustomSlotsAsync(doctorId, request);
                return Ok(new { Message = $"Created {slots.Count()} custom slots", Slots = slots });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/appointmentslots/admin/weekly
        [HttpPost("admin/weekly")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<AppointmentSlotResponse>>> CreateWeeklySlotsForDoctor(
            [FromBody] CreateWeeklySlotsRequest request)
        {
            try
            {
                var slots = await _slotService.CreateWeeklySlotsAsync(request);
                return Ok(new { Message = $"Created {slots.Count()} slots for the next 7 days", Slots = slots });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH: api/appointmentslots/5/unavailable
        [HttpPatch("{id}/unavailable")]
        [Authorize(Roles = "Doctor,Administrator")]
        public async Task<ActionResult<AppointmentSlotResponse>> MarkAsUnavailable(int id)
        {
            try
            {
                var slot = await _slotService.MarkSlotAsUnavailableAsync(id);
                
                if (User.IsInRole("Doctor"))
                {
                    var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    if (slot.DoctorId != doctorId)
                        return Forbid();
                }

                return Ok(slot);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/appointmentslots/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor,Administrator")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            try
            {
                if (User.IsInRole("Doctor"))
                {
                    var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    var slot = await _slotService.GetDoctorSlotsAsync(doctorId);
                    if (!slot.Any(s => s.Id == id))
                        return Forbid();
                }

                var result = await _slotService.DeleteSlotAsync(id);
                if (!result)
                    return NotFound("Slot not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}