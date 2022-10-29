namespace Imagegram.Controllers;

/// <param name="Name">Name of the file thath represent the image</param>
/// <param name="Uri">Uri at which image is available</param>
public record struct SavedImage(string Name, string Uri);