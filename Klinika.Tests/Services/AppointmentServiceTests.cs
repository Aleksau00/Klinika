using FluentAssertions;
using Klinika.DATA;
using Klinika.Models;
using Klinika.Models.DTOs;
using Klinika.Repositories;
using Klinika.Services;
using Klinika.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Klinika.Tests.Services
{
    public class AppointmentServiceTests
    {
        [Fact]
        public async Task BookAppointmentAsync_ShouldCreateTreatmentAppointmentAndMarkSlotUnavailable()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var appointmentRepo = new AppointmentRepository(context);
            var slotRepo = new AppointmentSlotRepository(context);
            var service = new AppointmentService(appointmentRepo, slotRepo, context);

            var request = new CreateAppointmentRequest
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                AppointmentType = AppointmentType.Treatment
            };

            // Act
            var result = await service.BookAppointmentAsync(request, secretaryId: 2);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(AppointmentStatus.Scheduled);
            result.AppointmentType.Should().Be(AppointmentType.Treatment);
            
            var slot = await context.AppointmentSlots.FindAsync(1);
            slot!.IsAvailable.Should().BeFalse();
        }

        [Fact]
        public async Task BookAppointmentAsync_ShouldThrowWhenSlotUnavailable()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var slot = await context.AppointmentSlots.FindAsync(1);
            slot!.IsAvailable = false;
            await context.SaveChangesAsync();

            var appointmentRepo = new AppointmentRepository(context);
            var slotRepo = new AppointmentSlotRepository(context);
            var service = new AppointmentService(appointmentRepo, slotRepo, context);

            var request = new CreateAppointmentRequest
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                AppointmentType = AppointmentType.Treatment
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.BookAppointmentAsync(request, secretaryId: 2)
            );
            exception.Message.Should().Contain("no longer available");
        }

        [Fact]
        public async Task CheckInPatientAsync_ShouldChangeStatusToInProgress()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var appointmentRepo = new AppointmentRepository(context);
            var slotRepo = new AppointmentSlotRepository(context);
            var service = new AppointmentService(appointmentRepo, slotRepo, context);

            var request = new CreateAppointmentRequest
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                AppointmentType = AppointmentType.Treatment
            };
            var booked = await service.BookAppointmentAsync(request, secretaryId: 2);

            // Act
            var result = await service.CheckInPatientAsync(booked.Id);

            // Assert
            result.Status.Should().Be(AppointmentStatus.InProgress);
            result.CheckedInAt.Should().NotBeNull();
        }

        [Fact]
        public async Task CancelAppointmentAsync_ShouldThrowWhenLessThan48Hours()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var slot = await context.AppointmentSlots.FindAsync(1);
            slot!.Date = DateOnly.FromDateTime(DateTime.Today.AddHours(47));
            await context.SaveChangesAsync();

            var appointmentRepo = new AppointmentRepository(context);
            var slotRepo = new AppointmentSlotRepository(context);
            var service = new AppointmentService(appointmentRepo, slotRepo, context);

            var request = new CreateAppointmentRequest
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                AppointmentType = AppointmentType.Treatment
            };
            var booked = await service.BookAppointmentAsync(request, secretaryId: 2);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CancelAppointmentAsync(booked.Id, "Test reason")
            );
            exception.Message.Should().Contain("48 hours");
        }

        [Fact]
        public async Task CancelAppointmentAsync_ShouldSucceedAndFreeSlot()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var appointmentRepo = new AppointmentRepository(context);
            var slotRepo = new AppointmentSlotRepository(context);
            var service = new AppointmentService(appointmentRepo, slotRepo, context);

            var request = new CreateAppointmentRequest
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                AppointmentType = AppointmentType.Treatment
            };
            var booked = await service.BookAppointmentAsync(request, secretaryId: 2);

            // Act
            var result = await service.CancelAppointmentAsync(booked.Id, "Patient request");

            // Assert
            result.Should().BeTrue();
            
            var appointment = await context.Appointments.FindAsync(booked.Id);
            appointment!.Status.Should().Be(AppointmentStatus.Cancelled);
            
            var slot = await context.AppointmentSlots.FindAsync(1);
            slot!.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public async Task MarkNoShowAsync_ShouldIncrementCountAndFreeSlot()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var appointmentRepo = new AppointmentRepository(context);
            var slotRepo = new AppointmentSlotRepository(context);
            var service = new AppointmentService(appointmentRepo, slotRepo, context);

            var request = new CreateAppointmentRequest
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                AppointmentType = AppointmentType.Treatment
            };
            var booked = await service.BookAppointmentAsync(request, secretaryId: 2);

            // Act
            await service.MarkNoShowAsync(booked.Id);

            // Assert
            var patient = await context.Patients.FindAsync(3);
            patient!.NoShowCount.Should().Be(1);
            
            var appointment = await context.Appointments.FindAsync(booked.Id);
            appointment!.Status.Should().Be(AppointmentStatus.NoShow);
            
            var slot = await context.AppointmentSlots.FindAsync(1);
            slot!.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public async Task CompleteTreatmentAppointmentAsync_ShouldFillAllMedicalFields()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var appointmentRepo = new AppointmentRepository(context);
            var slotRepo = new AppointmentSlotRepository(context);
            var service = new AppointmentService(appointmentRepo, slotRepo, context);

            var request = new CreateAppointmentRequest
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                AppointmentType = AppointmentType.Treatment
            };
            var booked = await service.BookAppointmentAsync(request, secretaryId: 2);
            await service.CheckInPatientAsync(booked.Id);

            var completionRequest = new CompleteTreatmentRequest
            {
                Anamnesis = "Patient has chest pain for two days",
                StatusObservation = "BP elevated, heart sounds normal",
                Therapy = "Prescribed medication and lifestyle changes",
                DiagnosedCondition = "Hypertension"
            };

            // Act
            var result = await service.CompleteTreatmentAppointmentAsync(booked.Id, completionRequest);

            // Assert
            result.Status.Should().Be(AppointmentStatus.Completed);
            result.Anamnesis.Should().Be("Patient has chest pain for two days");
            result.CompletedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task CompletePreventiveAppointmentAsync_ShouldFillNotes()
        {
            // Arrange
            var context = TestDbContextFactory.CreateContextWithData();
            var appointmentRepo = new AppointmentRepository(context);
            var slotRepo = new AppointmentSlotRepository(context);
            var service = new AppointmentService(appointmentRepo, slotRepo, context);

            var request = new CreateAppointmentRequest
            {
                AppointmentSlotId = 1,
                PatientId = 3,
                AppointmentType = AppointmentType.Preventive
            };
            var booked = await service.BookAppointmentAsync(request, secretaryId: 2);
            await service.CheckInPatientAsync(booked.Id);

            var completionRequest = new CompletePreventiveRequest
            {
                PreventiveNotes = "Annual checkup completed. All vitals normal."
            };

            // Act
            var result = await service.CompletePreventiveAppointmentAsync(booked.Id, completionRequest);

            // Assert
            result.Status.Should().Be(AppointmentStatus.Completed);
            result.PreventiveNotes.Should().Be("Annual checkup completed. All vitals normal.");
            result.CompletedAt.Should().NotBeNull();
        }
    }
}