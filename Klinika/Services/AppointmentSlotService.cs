using Klinika.Models;
using Klinika.Models.DTOs;
using Klinika.Repositories;

namespace Klinika.Services
{
    public class AppointmentSlotService : IAppointmentSlotService
    {
        private readonly IAppointmentSlotRepository _slotRepository;
        private readonly IWorkerRepository _workerRepository;

        public AppointmentSlotService(IAppointmentSlotRepository slotRepository, IWorkerRepository workerRepository)
        {
            _slotRepository = slotRepository;
            _workerRepository = workerRepository;
        }

        public async Task<IEnumerable<AppointmentSlotResponse>> GetDoctorSlotsAsync(int doctorId, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            var slots = await _slotRepository.GetSlotsByDoctorAsync(doctorId, fromDate, toDate);
            return slots.Select(MapToResponse);
        }

        public async Task<IEnumerable<AppointmentSlotResponse>> GetAvailableSlotsAsync(int doctorId, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            var slots = await _slotRepository.GetAvailableSlotsByDoctorAsync(doctorId, fromDate, toDate);
            return slots.Select(MapToResponse);
        }

        public async Task<AppointmentSlotResponse> CreateSlotAsync(int doctorId, DateOnly date, TimeOnly startTime)
        {
            // Validate doctor exists
            var doctor = await _workerRepository.GetByIdAsync(doctorId);
            if (doctor == null || doctor is not Doctor)
                throw new ArgumentException("Invalid doctor ID");

            // Create slot with 15-minute duration
            var slot = new AppointmentSlot
            {
                DoctorId = doctorId,
                Date = date,
                StartTime = startTime,
                EndTime = startTime.AddMinutes(AppointmentSlot.SlotDurationMinutes),
                IsAvailable = true
            };

            // Validate slot
            if (!slot.IsValidSlot())
                throw new ArgumentException("Invalid slot time. Slots must be in 15-minute intervals between 8:00 AM and 8:00 PM");

            // Check if slot already exists
            if (await _slotRepository.SlotExistsAsync(doctorId, date, startTime))
                throw new InvalidOperationException("Slot already exists for this time");

            var createdSlot = await _slotRepository.CreateSlotAsync(slot);
            createdSlot.Doctor = doctor as Doctor;
            return MapToResponse(createdSlot);
        }

        public async Task<IEnumerable<AppointmentSlotResponse>> CreateWeeklySlotsAsync(CreateWeeklySlotsRequest request)
        {
            // Validate doctor exists
            var doctor = await _workerRepository.GetByIdAsync(request.DoctorId);
            if (doctor == null || doctor is not Doctor)
                throw new ArgumentException("Invalid doctor ID");

            var slots = new List<AppointmentSlot>();
            var startDate = DateOnly.FromDateTime(DateTime.Today);

            // Generate slots for the next 7 days
            for (int day = 0; day < 7; day++)
            {
                var currentDate = startDate.AddDays(day);

                // Generate all 15-minute slots from 8 AM to 8 PM
                var currentTime = AppointmentSlot.WorkDayStart;
                while (currentTime < AppointmentSlot.WorkDayEnd)
                {
                    // Check if slot already exists
                    if (!await _slotRepository.SlotExistsAsync(request.DoctorId, currentDate, currentTime))
                    {
                        slots.Add(new AppointmentSlot
                        {
                            DoctorId = request.DoctorId,
                            Date = currentDate,
                            StartTime = currentTime,
                            EndTime = currentTime.AddMinutes(AppointmentSlot.SlotDurationMinutes),
                            IsAvailable = true
                        });
                    }

                    currentTime = currentTime.AddMinutes(AppointmentSlot.SlotDurationMinutes);
                }
            }

            if (slots.Any())
            {
                await _slotRepository.CreateSlotsAsync(slots);
                slots.ForEach(s => s.Doctor = doctor as Doctor);
            }

            return slots.Select(MapToResponse);
        }

        public async Task<IEnumerable<AppointmentSlotResponse>> CreateCustomSlotsAsync(int doctorId, CreateCustomSlotsRequest request)
        {
            // Validate doctor exists
            var doctor = await _workerRepository.GetByIdAsync(doctorId);
            if (doctor == null || doctor is not Doctor)
                throw new ArgumentException("Invalid doctor ID");

            // Validate time range
            if (request.StartTime >= request.EndTime)
                throw new ArgumentException("Start time must be before end time");

            if (request.StartTime < AppointmentSlot.WorkDayStart || request.EndTime > AppointmentSlot.WorkDayEnd)
                throw new ArgumentException($"Times must be between {AppointmentSlot.WorkDayStart} and {AppointmentSlot.WorkDayEnd}");

            var slots = new List<AppointmentSlot>();
            var datesToGenerate = new List<DateOnly>();

            // Determine which dates to generate slots for
            if (request.Dates != null && request.Dates.Any())
            {
                // Use specific dates provided
                datesToGenerate = request.Dates;
            }
            else
            {
                // Generate for next N days
                var startDate = DateOnly.FromDateTime(DateTime.Today);
                for (int day = 0; day < request.DaysAhead; day++)
                {
                    var currentDate = startDate.AddDays(day);
                    
                    // Skip weekends if requested
                    if (request.SkipWeekends && (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday))
                        continue;

                    datesToGenerate.Add(currentDate);
                }
            }

            // Generate slots for each date
            foreach (var date in datesToGenerate)
            {
                var currentTime = request.StartTime;
                while (currentTime < request.EndTime)
                {
                    // Check if slot already exists
                    if (!await _slotRepository.SlotExistsAsync(doctorId, date, currentTime))
                    {
                        slots.Add(new AppointmentSlot
                        {
                            DoctorId = doctorId,
                            Date = date,
                            StartTime = currentTime,
                            EndTime = currentTime.AddMinutes(AppointmentSlot.SlotDurationMinutes),
                            IsAvailable = true
                        });
                    }

                    currentTime = currentTime.AddMinutes(AppointmentSlot.SlotDurationMinutes);
                }
            }

            if (slots.Any())
            {
                await _slotRepository.CreateSlotsAsync(slots);
                slots.ForEach(s => s.Doctor = doctor as Doctor);
            }

            return slots.Select(MapToResponse);
        }

        public async Task<AppointmentSlotResponse> MarkSlotAsUnavailableAsync(int slotId)
        {
            var slot = await _slotRepository.GetByIdAsync(slotId);
            if (slot == null)
                throw new ArgumentException("Slot not found");

            slot.IsAvailable = false;
            var updatedSlot = await _slotRepository.UpdateSlotAsync(slot);
            return MapToResponse(updatedSlot);
        }

        public async Task<bool> DeleteSlotAsync(int slotId)
        {
            return await _slotRepository.DeleteSlotAsync(slotId);
        }

        private AppointmentSlotResponse MapToResponse(AppointmentSlot slot)
        {
            return new AppointmentSlotResponse
            {
                Id = slot.Id,
                DoctorId = slot.DoctorId,
                DoctorName = slot.Doctor != null ? $"{slot.Doctor.FirstName} {slot.Doctor.LastName}" : string.Empty,
                Date = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                IsAvailable = slot.IsAvailable
            };
        }
    }
}