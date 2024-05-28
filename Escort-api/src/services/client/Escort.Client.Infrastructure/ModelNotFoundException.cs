namespace Escort.Client.Infrastructure;

public class ModelNotFoundException : Exception
{
    public ModelNotFoundException() : base("Cannot find model in context.")
    {
        
    }
}