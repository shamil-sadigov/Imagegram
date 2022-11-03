## Imagegram

API that allows you to upload posts with images and comment on them

### Tech stack
- ASP.NET 6 API  
- EF/SQL Server 
- Azure Blob Storage (for saving images)

## Architecture decision records
- [Decision on choosing database SQL vs NoSQL](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20Database.SQL%20vs%20NoSQL.md)
- [Decision on sync vs async image uploading](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20sync%20vs%20async%20image%20uploading.md)
- [How cursor-based navigation is implemented](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20cursor-based-navigation.md)


## How to use API

### USERS

Before creating and commenting posts, you need to register a user and get user access token. 
When creating user, there is not any strong password policies, it just should not be less than 5 characters.

```py

# Register new user
POST api/v1/users
{
  "email": "some@email.com",
  "password": "pass@#"
}


# Get user access token (which is JWT)
POST api/v1/users/access-token
{
  "email": "some@email.com",
  "password": "pass@#"
}

```

Now you can create/get post, add/delete comments on posts, and retrieve posts [through pagination](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20cursor-based-navigation.md#how-it-works). 
But be sure to specify access token in HTTP header.

`Authorization: Bearer {ACEESS_TOKEN}`


### POSTS

```py

# Create a post by specifying Description and Image of the post
POST api/v1/posts  

# Get post without comments
GET api/v1/posts/{postId} 

# Get post with comments
GET api/v1/posts/{postId}?includeComments=true

# Get first page of posts where page size = 5. (PS: Max allowed page size is 50)
GET api/v1/posts?limit=5 

# Get to the next page.
GET api/v1/posts?limit=5&after={endCursor}

# Get to the previous page.
GET api/v1/posts?limit=5&after={startCursor}

```

### COMMENTS

```py

# Leave comment on post
POST api/v1/posts/{postId}/comment

# Delete comment from the post. (PS: Only user who created comment can delete it)
DELETE api/v1/posts/{postId}/comment/{commentId}

```

## How to run/deploy

Set connection string for SQL Server and Azure Blob Storage in `appsettings.json`


```
"ConnectionStrings": {
    "Database": "",
    "BlobStorage": ""
  }
```

ℹ️ No need to create empty database in SQL Server or containers in Blob Storage. It will be done on application startup. 

Now app can be deployed on Azure (or any other infrastructure).

## How to run unit tests

There are some integration tests with Database.

So if you want to run them, be sure that you specified connection string for SQL Server in `test-settings.json`

```
{
    "SqlServerConnectionString": "..."
}
```

I know, it's not convenient to be dependent on local environment when it comes to integration tests. 
It's better to wrap them into docker-compose and run all dependents services in docker. 
But for now, it's not implemented. Kindly ask you to suffer a little)


## Future improvements

- We can add distributed cache (Redis) to improve response latency and lighten the load on database
- [Image uploading can be made async](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20sync%20vs%20async%20image%20uploading.md#asynchronous-approach-with-websockets)
- Improve cursor-based navigation by trying [this solution](https://phauer.com/2017/web-api-pagination-continuation-token/) to prevent the [problem of dynamically chaning posts](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20cursor-based-navigation.md#problem)
- We can configure CDN for images, to serve images faster

### Notes

In order to keep project simple and not bloated, I skipped some points.

- EF migrations not added
- No logging
- Password protection is lightweight
- JWT has no expiration time
- No In memory caching

