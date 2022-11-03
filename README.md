## Imagegram

API that allows you to upload posts with images and comment on them

### Tech stack
- .NET 6 
- SQL Server
- Azure Blob Storage

## Architecture decision records
- Decision on choosing database SQL vs NoSQL
- Decision on sync vs async image uploading
- How cursor-based navigation work


## Why I've chosen SQL over NoSQL database

In my case, I decided to do some denormalization by storing number of Post comments in Post itself, and keep it in sync with actual number of comments.

And whenever I need to know the number of comments in a post, I just can look at `CommentCount` column in Post without having to to run resource-intensive counting operation (`GROUP BY, COUNT()`).

![image](https://user-images.githubusercontent.com/36125138/199615492-78fae1c6-ae88-4a95-8fbd-4fb18f7922a4.png)

### Anomaly

But here is a problem. Without proper transaction isolation I may encounter lost-update anomaly in concurrent scenario.

- Two concurrent transaction might get Post which `CommentCount = 0`
- Both this transaction will insert new comment in Comments table and will increment `Post.CommentCount`
- Eventually we might see that Post have `CommentCount = 1` but actually there are two comments associated with this post

![Code models (4)](https://user-images.githubusercontent.com/36125138/199616261-fd2a1ef5-43c6-46d3-a77f-819b5fc31964.jpg)

This problem can be solved by pessimistic/optimistic locking patterns, such as making all operations in one transaction with REPETABLEREAD Isolation level OR just by making `SELECT ... FOR UPDATE`

![Code models (5)](https://user-images.githubusercontent.com/36125138/199618243-237e54d3-bab0-4be2-a57a-de0cf44fb600.jpg)

Well, I'm inclined to go with SQL, because its ACID properties allow my data to keep consistent.


## Decision on sync vs async image uploading

There are multiple ways to create post with image uploading. Mainly it's divited into synchronous and asynchronous approaches

## Synchronous approach

- Client send a POST request to create a post with image
- API uploads the image to BlobStorage, saves Post in DB
- API return newly create post in response to Client

This approach is simplest design, but drawback is that we force the client to wait until image is processed and uploaded 

![Code models (8)](https://user-images.githubusercontent.com/36125138/199660965-6bfaf902-215e-40d5-9a0c-2d9636952a7b.jpg)


## [Asynchronous Request-Reply pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/async-request-reply)

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


## How to use API


## How to deploy


## Future improvements
- Distributed cache
- Make image uploading async
- Improve cursor-based navigation
- Add CDN for images

