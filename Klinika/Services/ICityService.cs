using Klinika.Models.DTOs;

namespace Klinika.Services
{
    public interface ICityService
    {
        Task<IEnumerable<CityDto>> GetAllCitiesAsync();
        Task<CityDto?> GetCityByIdAsync(int id);
        Task<CityDto> CreateCityAsync(CreateCityRequest request);
    }
}