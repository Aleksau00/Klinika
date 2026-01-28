using Klinika.Models.DTOs;
using Klinika.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Klinika.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicsController : ControllerBase
    {
        private readonly IClinicService _clinicService;

        public ClinicsController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicDto>>> GetAllClinics()
        {
            var clinics = await _clinicService.GetAllClinicsAsync();
            return Ok(clinics);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicDto>> GetClinicById(int id)
        {
            var clinic = await _clinicService.GetClinicByIdAsync(id);
            if (clinic == null)
                return NotFound($"Clinic with ID {id} not found");
            
            return Ok(clinic);
        }

        [HttpGet("{id}/workers")]
        public async Task<ActionResult<IEnumerable<WorkerDto>>> GetClinicWorkers(int id)
        {
            try
            {
                var workers = await _clinicService.GetClinicWorkersAsync(id);
                return Ok(workers);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ClinicDto>> CreateClinic([FromBody] CreateClinicRequest request)
        {
            try
            {
                var clinic = await _clinicService.CreateClinicAsync(request);
                return CreatedAtAction(nameof(GetClinicById), new { id = clinic.Id }, clinic);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ClinicDto>> UpdateClinic(int id, [FromBody] UpdateClinicRequest request)
        {
            try
            {
                var clinic = await _clinicService.UpdateClinicAsync(id, request);
                return Ok(clinic);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteClinic(int id)
        {
            var result = await _clinicService.DeleteClinicAsync(id);
            if (!result)
                return NotFound($"Clinic with ID {id} not found");
            
            return NoContent();
        }
    }
}