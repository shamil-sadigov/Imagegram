using System.Net.Mime;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ContentType = Azure.Core.ContentType;

namespace Imagegram.Features.Posts.Create;

public sealed class ImageStorage
{
    private readonly BlobServiceClient _blobServiceClient;

    public ImageStorage(BlobServiceClient blobServiceClient) 
        => _blobServiceClient = blobServiceClient;

    public Task<SavedImage> SaveOriginalImageAsync(string imageName, Stream stream) 
        => SaveImageToContainerAsync(imageName, stream, "original-images");

    public Task<SavedImage> SaveProcessedImageAsync(string imageName, Stream stream) 
        => SaveImageToContainerAsync(imageName, stream, "processed-images");

    private async Task<SavedImage> SaveImageToContainerAsync(string imageName, Stream stream, string containerName)
    {
        var rawImagesContainer = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient processedImage = rawImagesContainer.GetBlobClient(imageName);

        await processedImage.UploadAsync(stream, new BlobHttpHeaders()
        {
            ContentType ="image/jpeg"
        });
            
        return new SavedImage(processedImage.Name, processedImage.Uri.AbsoluteUri);;
            
    }
}