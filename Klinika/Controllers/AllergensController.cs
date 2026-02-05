using Klinika.Models.DTOs;
using Klinika.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Klinika.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AllergensController : ControllerBase
    {
        private readonly IAllergenService _allergenService;

        public AllergensController(IAllergenService allergenService)
        {
            _allergenService = allergenService;
        }

        /// <summary>
        /// Get all available allergens in the system
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Doctor,Secretary,Administrator")]
        public async Task<ActionResult<IEnumerable<AllergenDto>>> GetAllAllergens()
        {
            var allergens = await _allergenService.GetAllAllergensAsync();
            return Ok(allergens);
        }

        /// <summary>
        /// Get a specific allergen by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor,Secretary,Administrator")]
        public async Task<ActionResult<AllergenDto>> GetAllergenById(int id)
        {
            var allergen = await _allergenService.GetAllergenByIdAsync(id);
            if (allergen == null)
                return NotFound($"Allergen with ID {id} not found");

            return Ok(allergen);
        }

        /// <summary>
        /// Create a new allergen
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Doctor,Administrator")]
        public async Task<ActionResult<AllergenDto>> CreateAllergen([FromBody] CreateAllergenRequest request)
        {
            try
            {
                var allergen = await _allergenService.CreateAllergenAsync(request);
                return CreatedAtAction(nameof(GetAllergenById), new { id = allergen.Id }, allergen);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an allergen
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Administrator")]
        public async Task<ActionResult<AllergenDto>> UpdateAllergen(int id, [FromBody] CreateAllergenRequest request)
        {
            try
            {
                var allergen = await _allergenService.UpdateAllergenAsync(id, request);
                return Ok(allergen);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Delete an allergen
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteAllergen(int id)
        {
            var success = await _allergenService.DeleteAllergenAsync(id);
            if (!success)
                return NotFound($"Allergen with ID {id} not found");

            return NoContent();
        }

        // ===== PATIENT-ALLERGEN ENDPOINTS =====

        /// <summary>
        /// Get all allergens for a specific patient
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Doctor,Secretary,Administrator")]
        public async Task<ActionResult<IEnumerable<PatientAllergenDto>>> GetPatientAllergens(int patientId)
        {
            var allergens = await _allergenService.GetPatientAllergensAsync(patientId);
            return Ok(allergens);
        }

        /// <summary>
        /// Add an allergen to a patient
        /// </summary>
        [HttpPost("patient")]
        [Authorize(Roles = "Doctor,Secretary,Administrator")]
        public async Task<ActionResult<PatientAllergenDto>> AddAllergenToPatient([FromBody] AddPatientAllergenRequest request)
        {
            try
            {
                var patientAllergen = await _allergenService.AddAllergenToPatientAsync(request);
                return Ok(patientAllergen);
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
        /// Update patient allergen details (diagnosed date, notes)
        /// </summary>
        [HttpPut("patient/{patientId}/allergen/{allergenId}")]
        [Authorize(Roles = "Doctor,Secretary,Administrator")]
        public async Task<ActionResult<PatientAllergenDto>> UpdatePatientAllergen(
            int patientId, 
            int allergenId, 
            [FromBody] UpdatePatientAllergenRequest request)
        {
            try
            {
                var patientAllergen = await _allergenService.UpdatePatientAllergenAsync(patientId, allergenId, request);
                return Ok(patientAllergen);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Remove an allergen from a patient
        /// </summary>
        [HttpDelete("patient/{patientId}/allergen/{allergenId}")]
        [Authorize(Roles = "Doctor,Administrator")]
        public async Task<ActionResult> RemoveAllergenFromPatient(int patientId, int allergenId)
        {
            var success = await _allergenService.RemoveAllergenFromPatientAsync(patientId, allergenId);
            if (!success)
                return NotFound("Patient allergen association not found");

            return NoContent();
        }
    }
}