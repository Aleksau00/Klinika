    namespace Klinika.Models.DTOs
{
    public class VaccinationRecordDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int VaccinationId { get; set; }
        public string VaccinationName { get; set; }
        public DateTime AdministeredDate { get; set; }
        public string? Notes { get; set; }
        public int AdministeredByDoctorId { get; set; }
        public string AdministeredByDoctorName { get; set; }
    }
}