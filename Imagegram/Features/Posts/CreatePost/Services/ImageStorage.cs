﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Imagegram.Features.Posts.CreatePost.Services;

public sealed class ImageStorage : IImageStorage
{
    private readonly BlobServiceClient _blobServiceClient;

    public ImageStorage(BlobServiceClient blobServiceClient) 
        => _blobServiceClient = blobServiceClient;

    public Task<SavedImage> SaveOriginalImageAsync(string imageName, Stream stream, CancellationToken token) 
        => SaveImageToContainerAsync(imageName, stream, "original-images", token);

    public Task<SavedImage> SaveProcessedImageAsync(string imageName, Stream stream, CancellationToken token) 
        => SaveImageToContainerAsync(imageName, stream, "processed-images", token);

    private async Task<SavedImage> SaveImageToContainerAsync(
        string imageName, 
        Stream stream, 
        string containerName,
        CancellationToken cancellationToken)
    {
        var rawImagesContainer = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient processedImage = rawImagesContainer.GetBlobClient(imageName);

        await processedImage.UploadAsync(stream, new BlobHttpHeaders()
        {
            ContentType ="image/jpeg"
        }, cancellationToken: cancellationToken);
            
        return new SavedImage(processedImage.Name, processedImage.Uri.AbsoluteUri);;
            
    }
}