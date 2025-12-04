namespace Escort.Booking.Infrastructure;

public class ModelNotFoundException : Exception
{
    public ModelNotFoundException(string message) : base(message)
    {
    }
}

