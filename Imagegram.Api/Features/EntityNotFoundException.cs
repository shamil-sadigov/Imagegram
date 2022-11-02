namespace Imagegram.Api.Features;

public class EntityNotFoundException:Exception
{
    public EntityNotFoundException(string message):base(message)
    {
        
    }
}