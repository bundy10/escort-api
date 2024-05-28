namespace Escort.Driver.Application.Services;

public interface IDriverService
{
    Task<IEnumerable<Domain.Models.Driver>> GetAllDriverAsync();
    Task<Domain.Models.Driver> GetDriverByIdAsync(int id);
    Task<Domain.Models.Driver> CreateDriverAsync(Domain.Models.DriverContactDetails driverContactDetails);
    Task<Domain.Models.Driver> UpdateDriverAsync(Domain.Models.DriverContactDetails driverContactDetails);
    Task<Domain.Models.Driver> DeleteDriverAsync(int id);
}