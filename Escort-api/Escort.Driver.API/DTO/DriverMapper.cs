namespace Escort.Driver.API.DTO;

public static class DriverMapper
{
    public static DriverGetDTO ToDto(this Domain.Models.Driver driver)
    {
        return new DriverGetDTO
        {
            Id = driver.Id,
            DriverContactDetails = driver.DriverContactDetails
        };
    }
    
    public static Domain.Models.Driver ToDomain(this DriverPostPutDto driverDto)
    {
        return new Domain.Models.Driver(driverDto.DriverContactDetails).WithId();
    }
}