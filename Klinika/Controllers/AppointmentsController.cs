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
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // === SECRETARY ENDPOINTS ===

        /// <summary>
        /// Book a new appointment (Secretary/Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Secretary,Administrator")]
        public async Task<ActionResult<AppointmentDto>> BookAppointment([FromBody] CreateAppointmentRequest request)
        {
            try
            {
                var secretaryId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var appointment = await _appointmentService.BookAppointmentAsync(request, secretaryId);
                return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get appointment details by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Secretary,Doctor,Administrator")]
        public async Task<ActionResult<AppointmentDto>> GetAppointmentById(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound($"Appointment with ID {id} not found");

            return Ok(appointment);
        }

        /// <summary>
        /// Get all appointments for a specific patient
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Secretary,Doctor,Administrator")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetPatientAppointments(int patientId)
        {
            var appointments = await _appointmentService.GetPatientAppointmentsAsync(patientId);
            return Ok(appointments);
        }

        /// <summary>
        /// Get doctor's schedule (optionally filtered by date)
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Secretary,Doctor,Administrator")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetDoctorSchedule(
            int doctorId, 
            [FromQuery] string? date = null)
        {
            DateOnly? parsedDate = null;
            if (!string.IsNullOrEmpty(date))
            {
                if (!DateOnly.TryParse(date, out var tempDate))
                    return BadRequest("Invalid date format. Use YYYY-MM-DD");
                parsedDate = tempDate;
            }

            var appointments = await _appointmentService.GetDoctorScheduleAsync(doctorId, parsedDate);
            return Ok(appointments);
        }

        /// <summary>
        /// Get clinic's schedule for a specific date
        /// </summary>
        [HttpGet("clinic/{clinicId}")]
        [Authorize(Roles = "Secretary,Administrator")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetClinicSchedule(
            int clinicId,
            [FromQuery] string date)
        {
            if (string.IsNullOrEmpty(date))
                return BadRequest("Date parameter is required. Use format: YYYY-MM-DD");

            if (!DateOnly.TryParse(date, out var parsedDate))
                return BadRequest("Invalid date format. Use YYYY-MM-DD");

            var appointments = await _appointmentService.GetClinicScheduleAsync(clinicId, parsedDate);
            return Ok(appointments);
        }

        /// <summary>
        /// Check in patient for appointment (moves from Scheduled to InProgress)
        /// </summary>
        [HttpPut("{id}/check-in")]
        [Authorize(Roles = "Secretary,Administrator")]
        public async Task<ActionResult<AppointmentDto>> CheckInPatient(int id)
        {
            try
            {
                var appointment = await _appointmentService.CheckInPatientAsync(id);
                return Ok(appointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cancel appointment (48-hour rule enforced)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Secretary,Administrator")]
        public async Task<ActionResult> CancelAppointment(int id, [FromBody] CancelAppointmentRequest request)
        {
            try
            {
                await _appointmentService.CancelAppointmentAsync(id, request.Reason);
                return Ok(new { Message = "Appointment cancelled successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Mark patient as no-show
        /// </summary>
        [HttpPut("{id}/mark-noshow")]
        [Authorize(Roles = "Secretary,Doctor,Administrator")]
        public async Task<ActionResult> MarkNoShow(int id)
        {
            try
            {
                await _appointmentService.MarkNoShowAsync(id);
                return Ok(new { Message = "Patient marked as no-show. No-show count incremented." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // === DOCTOR ENDPOINTS ===

        /// <summary>
        /// Complete treatment appointment with medical details
        /// </summary>
        [HttpPut("{id}/complete-treatment")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<AppointmentDto>> CompleteTreatmentAppointment(
            int id,
            [FromBody] CompleteTreatmentRequest request)
        {
            try
            {
                var appointment = await _appointmentService.CompleteTreatmentAppointmentAsync(id, request);
                return Ok(appointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Complete preventive appointment with notes
        /// </summary>
        [HttpPut("{id}/complete-preventive")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<AppointmentDto>> CompletePreventiveAppointment(
            int id,
            [FromBody] CompletePreventiveRequest request)
        {
            try
            {
                var appointment = await _appointmentService.CompletePreventiveAppointmentAsync(id, request);
                return Ok(appointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}