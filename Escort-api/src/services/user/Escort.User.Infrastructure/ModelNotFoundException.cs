namespace Escort.User.Infrastructure;

public class ModelNotFoundException : Exception
{
    public ModelNotFoundException() : base("Cannot find model in context.")
    {
        
    }
}