using Klinika.Models;

namespace Klinika.Repositories
{
    public interface IAddressRepository
    {
        Task<Address?> GetByIdAsync(int id);
        Task<Address> CreateAsync(Address address);
        Task<Address> UpdateAsync(Address address);
        Task<bool> DeleteAsync(int id);
    }
}