using Klinika.Models;
using Klinika.Models.DTOs;
using Klinika.Repositories;

namespace Klinika.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<IEnumerable<CityDto>> GetAllCitiesAsync()
        {
            var cities = await _cityRepository.GetAllAsync();
            return cities.Select(MapToDto);
        }

        public async Task<CityDto?> GetCityByIdAsync(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            return city != null ? MapToDto(city) : null;
        }

        public async Task<CityDto> CreateCityAsync(CreateCityRequest request)
        {
            // Check if city already exists
            var existing = await _cityRepository.GetByNameAsync(request.Name, request.Country);
            if (existing != null)
                throw new InvalidOperationException($"City {request.Name} in {request.Country} already exists");

            var city = new City
            {
                Name = request.Name,
                PostalCode = request.PostalCode,
                Country = request.Country
            };

            var created = await _cityRepository.CreateAsync(city);
            return MapToDto(created);
        }

        private CityDto MapToDto(City city)
        {
            return new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                PostalCode = city.PostalCode,
                Country = city.Country
            };
        }
    }
}