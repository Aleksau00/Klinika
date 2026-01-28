using Klinika.DATA;
using Klinika.Models;
using Microsoft.EntityFrameworkCore;

namespace Klinika.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Address?> GetByIdAsync(int id)
        {
            return await _context.Addresses
                .Include(a => a.City)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Address> CreateAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            
            // Load the City navigation property
            await _context.Entry(address).Reference(a => a.City).LoadAsync();
            
            return address;
        }

        public async Task<Address> UpdateAsync(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            
            // Load the City navigation property
            await _context.Entry(address).Reference(a => a.City).LoadAsync();
            
            return address;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}