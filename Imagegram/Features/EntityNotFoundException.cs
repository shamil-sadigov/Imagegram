namespace Imagegram.Features;

public class EntityNotFoundException:Exception
{
    public EntityNotFoundException(string message):base(message)
    {
        
    }
}