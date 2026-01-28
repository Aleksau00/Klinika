using Klinika.DATA;
using Klinika.Models;
using Microsoft.EntityFrameworkCore;

namespace Klinika.Repositories
{
    // Repositories/WorkerRepository.cs
    internal class WorkerRepository : IWorkerRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Worker> GetByIdAsync(int id)
        {
            return await _context.Workers.FindAsync(id);
        }

        public async Task<Worker> GetByEmailAsync(string email)
        {
            return await _context.Workers.FirstOrDefaultAsync(w => w.Email == email);
        }

        public async Task<Administrator> CreateAdministratorAsync(Administrator admin)
        {
            _context.Administrators.Add(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task<Doctor> CreateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task<Secretary> CreateSecretaryAsync(Secretary secretary)
        {
            _context.Secretaries.Add(secretary);
            await _context.SaveChangesAsync();
            return secretary;
        }



        public async Task<Worker> UpdateAsync(Worker worker)
        {
            _context.Workers.Update(worker);
            await _context.SaveChangesAsync();
            return worker;
        }
    }
}
