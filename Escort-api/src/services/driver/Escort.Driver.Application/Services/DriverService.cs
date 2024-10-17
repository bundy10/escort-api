using Escort.Driver.Application.Repositories;
using Escort.Driver.Domain.Models;

namespace Escort.Driver.Application.Services;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepository;

    public DriverService(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }
    
    public async Task<Domain.Models.Driver> CreateDriverAsync(DriverContactDetails driverContactDetails, string firstName, string lastName)
    {
        var driver = new Domain.Models.Driver()
        {
            FirstName = firstName,
            LastName = lastName,
            DriverContactDetails = driverContactDetails
        };
        return await _driverRepository.CreateAsync(driver);
    }
    
    public async Task<IEnumerable<Domain.Models.Driver>> GetAllDriverAsync()
    {
        return await _driverRepository.GetAllAsync();
    }
    
    public async Task<Domain.Models.Driver> GetDriverByIdAsync(int id)
    {
        return await _driverRepository.GetByIdAsync(id);
    }
    
    public async Task<Domain.Models.Driver> UpdateDriverAsync(DriverContactDetails driverContactDetails, string firstName, string lastName)
    {
        var driver = new Domain.Models.Driver()
        {
            FirstName = firstName,
            LastName = lastName,
            DriverContactDetails = driverContactDetails
        };
        return await _driverRepository.UpdateAsync(driver);
    }
    
    public async Task<Domain.Models.Driver> DeleteDriverAsync(int id)
    {
        return await _driverRepository.DeleteAsync(id);
    }
}