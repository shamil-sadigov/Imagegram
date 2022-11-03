## Imagegram

API that allows you to upload posts with images and comment on them

### Tech stack
- ASP.NET 6 API  
- EF/SQL Server 
- Azure Blob Storage

## Architecture decision records
- [Decision on choosing database SQL vs NoSQL](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20Database.SQL%20vs%20NoSQL.md)
- [Decision on sync vs async image uploading](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20sync%20vs%20async%20image%20uploading.md)
- [How cursor-based navigation is implemented](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20cursor-based-navigation.md)


## How to use API

![image](https://user-images.githubusercontent.com/36125138/199700836-3bfbe438-ce49-4922-a82f-9274b66d8179.png)

I believe that endpoints pretty intuitive and doesn't need comprehensive documentation.

Before creating and commenting posts, you need to register a user and get user access token.

Register user

```
POST api/v1/users
{
  "email": "some@email.com",
  "password": "pass@#"
}

```

Get user access token (JWT)

```
POST api/v1/users/access-token
{
  "email": "some@email.com",
  "password": "pass@#"
}

RESPONE
{
  "token": {ACEESS_TOKEN}
}

```

Now you can create/get post, add/delete comments on posts, and retrieve posts [through pagination](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20cursor-based-navigation.md#how-it-works). 
But be sure to specify access token in HTTP header.

`Authorization: Bearer {ACEESS_TOKEN}`

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

In order to keep project simple and not bloated, I skipped some points

- EF migrations not added
- No logging
- Password protection is lightweight
- JWT has no expiration time
- No In memory caching

