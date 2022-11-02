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

And whenever I need to know the number of comments in a post, there is no need to run resource-intensive counting operation, instead we can just see `CommentCount` column in Post.

![image](https://user-images.githubusercontent.com/36125138/199615492-78fae1c6-ae88-4a95-8fbd-4fb18f7922a4.png)

### Anomaly

But here is a problem. Without proper transaction isolation I may encounter lost-update anomaly.

![Code models (4)](https://user-images.githubusercontent.com/36125138/199616261-fd2a1ef5-43c6-46d3-a77f-819b5fc31964.jpg)


This problem can be solved by pessimistic/optimistic locking patterns, such as making all operations in one transaction with REPETABLEREAD Isolation level OR just by making `SELECT ... FOR UPDATE`

![Code models (5)](https://user-images.githubusercontent.com/36125138/199618243-237e54d3-bab0-4be2-a57a-de0cf44fb600.jpg)


So, I'm inclined to go with SQL, because its ACID properties allow my data to keep consistent.


## How to use API

## How to deploy


## Future improvements
- Distributed cache
- Make image uploading async
- Improve cursor-based navigation
- Add CDN for images

