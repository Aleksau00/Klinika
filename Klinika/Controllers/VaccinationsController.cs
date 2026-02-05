using Klinika.Models.DTOs;
using Klinika.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Klinika.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VaccinationsController : ControllerBase
    {
        private readonly IVaccinationService _vaccinationService;

        public VaccinationsController(IVaccinationService vaccinationService)
        {
            _vaccinationService = vaccinationService;
        }

        /// <summary>
        /// Get all available vaccinations
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Doctor,Secretary,Administrator")]
        public async Task<ActionResult<IEnumerable<VaccinationDto>>> GetAllVaccinations()
        {
            var vaccinations = await _vaccinationService.GetAllVaccinationsAsync();
            return Ok(vaccinations);
        }

        /// <summary>
        /// Get vaccination by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor,Secretary,Administrator")]
        public async Task<ActionResult<VaccinationDto>> GetVaccinationById(int id)
        {
            var vaccination = await _vaccinationService.GetVaccinationByIdAsync(id);
            if (vaccination == null)
                return NotFound($"Vaccination with ID {id} not found");

            return Ok(vaccination);
        }

        /// <summary>
        /// Create a new vaccination type
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<VaccinationDto>> CreateVaccination([FromBody] CreateVaccinationRequest request)
        {
            try
            {
                var vaccination = await _vaccinationService.CreateVaccinationAsync(request);
                return CreatedAtAction(nameof(GetVaccinationById), new { id = vaccination.Id }, vaccination);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update vaccination
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<VaccinationDto>> UpdateVaccination(int id, [FromBody] CreateVaccinationRequest request)
        {
            try
            {
                var vaccination = await _vaccinationService.UpdateVaccinationAsync(id, request);
                return Ok(vaccination);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Delete vaccination
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteVaccination(int id)
        {
            var success = await _vaccinationService.DeleteVaccinationAsync(id);
            if (!success)
                return NotFound($"Vaccination with ID {id} not found");

            return NoContent();
        }

        // ===== VACCINATION RECORDS =====

        /// <summary>
        /// Get patient vaccination records
        /// </summary>
        [HttpGet("records/patient/{patientId}")]
        [Authorize(Roles = "Doctor,Secretary,Administrator")]
        public async Task<ActionResult<IEnumerable<VaccinationRecordDto>>> GetPatientVaccinationRecords(int patientId)
        {
            var records = await _vaccinationService.GetPatientVaccinationRecordsAsync(patientId);
            return Ok(records);
        }

        /// <summary>
        /// Get vaccination record by ID
        /// </summary>
        [HttpGet("records/{id}")]
        [Authorize(Roles = "Doctor,Secretary,Administrator")]
        public async Task<ActionResult<VaccinationRecordDto>> GetVaccinationRecordById(int id)
        {
            var record = await _vaccinationService.GetVaccinationRecordByIdAsync(id);
            if (record == null)
                return NotFound($"Vaccination record with ID {id} not found");

            return Ok(record);
        }

        /// <summary>
        /// Administer vaccination (Doctor only)
        /// </summary>
        [HttpPost("records/administer")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<VaccinationRecordDto>> AdministerVaccination([FromBody] AdministerVaccinationRequest request)
        {
            try
            {
                var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var record = await _vaccinationService.AdministerVaccinationAsync(request, doctorId);
                return Ok(record);
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
        /// Delete vaccination record
        /// </summary>
        [HttpDelete("records/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteVaccinationRecord(int id)
        {
            var success = await _vaccinationService.DeleteVaccinationRecordAsync(id);
            if (!success)
                return NotFound($"Vaccination record with ID {id} not found");

            return NoContent();
        }
    }
}