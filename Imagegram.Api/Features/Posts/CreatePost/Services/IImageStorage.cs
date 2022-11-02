namespace Imagegram.Api.Features.Posts.CreatePost.Services;

public interface IImageStorage
{
    Task<SavedImage> SaveOriginalImageAsync(string imageName, Stream stream, CancellationToken token);
    Task<SavedImage> SaveProcessedImageAsync(string imageName, Stream stream, CancellationToken token);
}