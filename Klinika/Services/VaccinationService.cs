using Klinika.Models;
using Klinika.Models.DTOs;
using Klinika.Repositories;

namespace Klinika.Services
{
    public class VaccinationService : IVaccinationService
    {
        private readonly IVaccinationRepository _vaccinationRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public VaccinationService(
            IVaccinationRepository vaccinationRepository,
            IAppointmentRepository appointmentRepository)
        {
            _vaccinationRepository = vaccinationRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<VaccinationDto?> GetVaccinationByIdAsync(int id)
        {
            var vaccination = await _vaccinationRepository.GetByIdAsync(id);
            return vaccination == null ? null : MapToVaccinationDto(vaccination);
        }

        public async Task<IEnumerable<VaccinationDto>> GetAllVaccinationsAsync()
        {
            var vaccinations = await _vaccinationRepository.GetAllAsync();
            return vaccinations.Select(MapToVaccinationDto);
        }

        public async Task<VaccinationDto> CreateVaccinationAsync(CreateVaccinationRequest request)
        {
            var existing = await _vaccinationRepository.GetByNameAsync(request.Name);
            if (existing != null)
                throw new InvalidOperationException($"Vaccination '{request.Name}' already exists");

            var vaccination = new Vaccination
            {
                Name = request.Name,
                Description = request.Description
            };

            var created = await _vaccinationRepository.CreateAsync(vaccination);
            return MapToVaccinationDto(created);
        }

        public async Task<VaccinationDto> UpdateVaccinationAsync(int id, CreateVaccinationRequest request)
        {
            var vaccination = await _vaccinationRepository.GetByIdAsync(id);
            if (vaccination == null)
                throw new KeyNotFoundException("Vaccination not found");

            vaccination.Name = request.Name;
            vaccination.Description = request.Description;

            var updated = await _vaccinationRepository.UpdateAsync(vaccination);
            return MapToVaccinationDto(updated);
        }

        public async Task<bool> DeleteVaccinationAsync(int id)
        {
            return await _vaccinationRepository.DeleteAsync(id);
        }

        public async Task<VaccinationRecordDto?> GetVaccinationRecordByIdAsync(int id)
        {
            var record = await _vaccinationRepository.GetRecordByIdAsync(id);
            return record == null ? null : MapToVaccinationRecordDto(record);
        }

        public async Task<IEnumerable<VaccinationRecordDto>> GetPatientVaccinationRecordsAsync(int patientId)
        {
            var records = await _vaccinationRepository.GetPatientVaccinationRecordsAsync(patientId);
            return records.Select(MapToVaccinationRecordDto);
        }

        public async Task<VaccinationRecordDto> AdministerVaccinationAsync(AdministerVaccinationRequest request, int doctorId)
        {
            // Get the appointment
            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(request.PreventiveAppointmentId);
            if (appointment == null || appointment is not PreventiveAppointment preventiveAppointment)
                throw new KeyNotFoundException("Preventive appointment not found");

            // Verify appointment is completed or in progress
            if (preventiveAppointment.Status != AppointmentStatus.InProgress && 
                preventiveAppointment.Status != AppointmentStatus.Completed)
                throw new InvalidOperationException("Appointment must be in progress or completed to administer vaccination");

            // Verify the vaccination exists
            var vaccination = await _vaccinationRepository.GetByIdAsync(request.VaccinationId);
            if (vaccination == null)
                throw new KeyNotFoundException("Vaccination not found");

            // Create vaccination record
            var record = new VaccinationRecord
            {
                PatientId = preventiveAppointment.PatientId,
                VaccinationId = request.VaccinationId,
                AdministeredDate = DateTime.UtcNow,
                Notes = request.Notes,
                AdministeredByDoctorId = doctorId,
                PreventiveAppointmentId = request.PreventiveAppointmentId
            };

            var created = await _vaccinationRepository.CreateRecordAsync(record);

            // Update the preventive appointment to mark it as vaccination
            preventiveAppointment.IsVaccination = true;
            preventiveAppointment.VaccinationId = request.VaccinationId;
            await _appointmentRepository.UpdateAsync(preventiveAppointment);

            return MapToVaccinationRecordDto(created);
        }

        public async Task<bool> DeleteVaccinationRecordAsync(int id)
        {
            return await _vaccinationRepository.DeleteRecordAsync(id);
        }

        private static VaccinationDto MapToVaccinationDto(Vaccination vaccination)
        {
            return new VaccinationDto
            {
                Id = vaccination.Id,
                Name = vaccination.Name,
                Description = vaccination.Description
            };
        }

        private static VaccinationRecordDto MapToVaccinationRecordDto(VaccinationRecord record)
        {
            return new VaccinationRecordDto
            {
                Id = record.Id,
                PatientId = record.PatientId,
                PatientName = $"{record.Patient.FirstName} {record.Patient.LastName}",
                VaccinationId = record.VaccinationId,
                VaccinationName = record.Vaccination.Name,
                AdministeredDate = record.AdministeredDate,
                Notes = record.Notes,
                AdministeredByDoctorId = record.AdministeredByDoctorId,
                AdministeredByDoctorName = $"{record.AdministeredByDoctor.FirstName} {record.AdministeredByDoctor.LastName}"
            };
        }
    }
}