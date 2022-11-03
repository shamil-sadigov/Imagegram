
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
