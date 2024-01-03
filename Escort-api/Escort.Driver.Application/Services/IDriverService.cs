namespace Escort.Driver.Application.Services;

public interface IDriverService
{
    Task<IEnumerable<Domain.Models.Driver>> GetAllDriverAsync();
    Task<Domain.Models.Driver> GetDriverByIdAsync(Guid id);
    Task<Domain.Models.Driver> CreateDriverAsync(Domain.Models.DriverContactDetails driverContactDetails);
    Task<Domain.Models.Driver> UpdateDriverAsync(Domain.Models.DriverContactDetails driverContactDetails);
    Task<Domain.Models.Driver> DeleteDriverAsync(Guid id);
}