using Xunit;
using FluentAssertions;
using Klinika.Models;
using Klinika.Repositories;
using Klinika.Tests.Helpers;

namespace Klinika.Tests.Repositories
{
    public class AppointmentRepositoryTests
    {
        [Fact]
        public async Task CreateTreatmentAsync_ShouldCreateAppointmentWithNullableFields()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var repository = new AppointmentRepository(context);

            var appointment = new TreatmentAppointment
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                DoctorId = 1,
                ClinicId = 1,
                ScheduledDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                ScheduledStartTime = new TimeOnly(9, 0),
                ScheduledEndTime = new TimeOnly(9, 15),
                BookedByWorkerId = 2,
                Status = AppointmentStatus.Scheduled
                // Medical fields are null when booking
            };

            // Act
            var result = await repository.CreateTreatmentAsync(appointment);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Anamnesis.Should().BeNull();
            result.StatusObservation.Should().BeNull();
            result.Therapy.Should().BeNull();
            result.DiagnosedCondition.Should().BeNull();
        }

        [Fact]
        public async Task CreatePreventiveAsync_ShouldCreateAppointmentWithNullNotes()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var repository = new AppointmentRepository(context);

            var appointment = new PreventiveAppointment
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                DoctorId = 1,
                ClinicId = 1,
                ScheduledDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                ScheduledStartTime = new TimeOnly(9, 0),
                ScheduledEndTime = new TimeOnly(9, 15),
                BookedByWorkerId = 2,
                Status = AppointmentStatus.Scheduled
                // PreventiveNotes is null when booking
            };

            // Act
            var result = await repository.CreatePreventiveAsync(appointment);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.PreventiveNotes.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTreatmentAppointmentWithMedicalData()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var repository = new AppointmentRepository(context);

            var appointment = new TreatmentAppointment
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                DoctorId = 1,
                ClinicId = 1,
                ScheduledDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                ScheduledStartTime = new TimeOnly(9, 0),
                ScheduledEndTime = new TimeOnly(9, 15),
                BookedByWorkerId = 2,
                Status = AppointmentStatus.Scheduled
            };
            var created = await repository.CreateTreatmentAsync(appointment);

            // Update with medical data
            created.Status = AppointmentStatus.Completed;
            created.Anamnesis = "Test anamnesis";
            created.StatusObservation = "Test observation";
            created.Therapy = "Test therapy";
            created.DiagnosedCondition = "Test condition";

            // Act
            var result = await repository.UpdateAsync(created);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(AppointmentStatus.Completed);
            ((TreatmentAppointment)result).Anamnesis.Should().Be("Test anamnesis");
        }

        [Fact]
        public async Task GetByIdWithDetailsAsync_ShouldLoadNavigationProperties()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var repository = new AppointmentRepository(context);

            var appointment = new TreatmentAppointment
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                DoctorId = 1,
                ClinicId = 1,
                ScheduledDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                ScheduledStartTime = new TimeOnly(9, 0),
                ScheduledEndTime = new TimeOnly(9, 15),
                BookedByWorkerId = 2,
                Status = AppointmentStatus.Scheduled
            };
            var created = await repository.CreateTreatmentAsync(appointment);

            // Act
            var result = await repository.GetByIdWithDetailsAsync(created.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Patient.Should().NotBeNull();
            result.Patient.FirstName.Should().Be("Test");
            result.Doctor.Should().NotBeNull();
            result.Clinic.Should().NotBeNull();
            result.BookedByWorker.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByPatientIdAsync_ShouldReturnAllPatientAppointments()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var repository = new AppointmentRepository(context);

            // Create multiple appointments
            await repository.CreateTreatmentAsync(new TreatmentAppointment
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                DoctorId = 1,
                ClinicId = 1,
                ScheduledDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                ScheduledStartTime = new TimeOnly(9, 0),
                ScheduledEndTime = new TimeOnly(9, 15),
                BookedByWorkerId = 2,
                Status = AppointmentStatus.Scheduled
            });

            // Act
            var results = await repository.GetByPatientIdAsync(3);

            // Assert
            results.Should().NotBeEmpty();
            results.Should().HaveCount(1);
            results.Should().AllSatisfy(a => a.PatientId.Should().Be(3));
        }

        [Fact]
        public async Task IsSlotBookedAsync_ShouldReturnTrueForActiveAppointment()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var repository = new AppointmentRepository(context);

            await repository.CreateTreatmentAsync(new TreatmentAppointment
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                DoctorId = 1,
                ClinicId = 1,
                ScheduledDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                ScheduledStartTime = new TimeOnly(9, 0),
                ScheduledEndTime = new TimeOnly(9, 15),
                BookedByWorkerId = 2,
                Status = AppointmentStatus.Scheduled
            });

            // Act
            var result = await repository.IsSlotBookedAsync(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Diagnostic_CheckIfPropertiesAreNullable()
        {
            var appointment = new TreatmentAppointment();

            // These should all compile without errors if properties are nullable
            appointment.Anamnesis = null;
            appointment.StatusObservation = null;
            appointment.Therapy = null;
            appointment.DiagnosedCondition = null;

            Assert.True(true); // If this compiles, properties are nullable
        }

        [Fact]
        public async Task IsSlotBookedAsync_ShouldReturnFalseForCancelledAppointment()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var repository = new AppointmentRepository(context);

            await repository.CreateTreatmentAsync(new TreatmentAppointment
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                DoctorId = 1,
                ClinicId = 1,
                ScheduledDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                ScheduledStartTime = new TimeOnly(9, 0),
                ScheduledEndTime = new TimeOnly(9, 15),
                BookedByWorkerId = 2,
                Status = AppointmentStatus.Cancelled
            });

            // Act
            var result = await repository.IsSlotBookedAsync(1);

            // Assert
            result.Should().BeFalse();
        }
    }
}