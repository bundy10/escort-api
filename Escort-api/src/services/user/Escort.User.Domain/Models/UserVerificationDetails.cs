namespace Escort.User.Domain.Models;

public class UserVerificationDetails : BaseDomainModel
{
    public string? DriversLicenseNumber { get; set; }
    public string? PassportNumber { get; set; }
    public string? Abn { get; set; }
    private bool _isVerified;

    private bool Verify()
    {
        int verificationPoints = 0;
        if (DriversLicenseNumber != null && PassportNumber != null)
        {
            verificationPoints += 2;
        }

        _isVerified = verificationPoints == 2;
        return _isVerified;
    }
}

