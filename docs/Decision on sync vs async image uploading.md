
## Decision on sync vs async image uploading

There are multiple ways to create post with image uploading. Mainly it's divited into synchronous and asynchronous approaches

### Synchronous approach

- Client send a POST request to create a post with image
- API uploads the image to BlobStorage, saves Post in DB
- API return newly create post in response to Client

This approach is simplest design, but drawback is that we force the client to wait until image is processed and uploaded 

![Code models (8)](https://user-images.githubusercontent.com/36125138/199660965-6bfaf902-215e-40d5-9a0c-2d9636952a7b.jpg)


### [Asynchronous Request-Reply pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/async-request-reply)

- Client send a POST request to create a post with image
- API uploads the image to BlobStorage, and triggers azure-function to process (resize & convert) the image
- API creates a Job that keep the status of image processing operation and return it to Client
- Client check status of the Job from time to time by polling API, once Job is completed, Client will provided with ID of the newly created Post

This approach is more advantageous, because client is not blocked synchnously by waiting for post creation, 
BUT this approach is more complicated than previous synchronous one, it can complicate both client and API, polling-based technique it's more resource-consuming and client is not immediately notified about job completion, so there can be some lag.

![Code models (12)](https://user-images.githubusercontent.com/36125138/199669459-f4e03f9c-3325-4500-a5c6-b23e58627f51.jpg)

### Asynchronous approach with websockets

This is similar to previous Asynchronous Request-Reply pattern, but now client doesnt't need to do polling of status of the job, instead client just has stable websocket connection to API server, and once API completed image peorcessing and created post, API can notify Client about it via websockets. 

This approach is more advantageous, because client is not blocked synchnously by waiting for post creation, it's less resource-consuming and client is immediately notified once post is created. BUT this approach is more complicated and labor-intensitve.


### So, what I've chosen

I decided to go with simplest synchronous approach in order to not complicate design and deliver feature faster, and move towards asynchrony only in case when it really required.
