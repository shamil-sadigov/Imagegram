using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Imagegram.Controllers;

public sealed class PostImageStorage
{
    private readonly BlobServiceClient _blobServiceClient;

    public PostImageStorage(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<SavedImage> SaveOriginalImageAsync(string imageName, Stream stream)
    {
        return await SaveImageToContainerAsync(imageName, stream, "original-images");
    }
        
    public async Task<SavedImage> SaveProcessedImageAsync(string imageName, Stream stream)
    {
        return await SaveImageToContainerAsync(imageName, stream, "processed-images");
    }

    private async Task<SavedImage> SaveImageToContainerAsync(string imageName, Stream stream, string containername)
    {
        var rawImagesContainer = _blobServiceClient.GetBlobContainerClient("processed-images");
        BlobClient processedImage = rawImagesContainer.GetBlobClient(imageName);

        await processedImage.UploadAsync(stream, new BlobHttpHeaders()
        {
            ContentType = "image/jpeg"
        });
            
        return new SavedImage(processedImage.Name, processedImage.Uri.AbsoluteUri);;
            
    }
}