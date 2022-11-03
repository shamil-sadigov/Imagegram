## Imagegram

API that allows you to upload posts with images and comment on them

### Tech stack
- .NET 6 
- SQL Server
- Azure Blob Storage

## Architecture decision records
- [Decision on choosing database SQL vs NoSQL](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20Database.SQL%20vs%20NoSQL.md)
- [Decision on sync vs async image uploading](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20sync%20vs%20async%20image%20uploading.md)
- [How cursor-based navigation work](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20cursor-based-navigation.md)


## How to use API

![image](https://user-images.githubusercontent.com/36125138/199700836-3bfbe438-ce49-4922-a82f-9274b66d8179.png)

Before creating and commenting posts, you need to register a user.

Register user

```
POST api/v1/users
{
  "email": "some@email.com",
  "password": "pass@#"
}

RESPONE 200

```

Get user access token (JWT)

```
POST api/v1/users/access-token

{
  "email": "some@email.com",
  "password": "pass@#"
}

RESPONE 200
{
  "token": {ACEESS_TOKEN}
}

```

Now you can create/get post, add/delete comments on posts. But be sure to specify access token in HTTP header.
`Authorization: Bearer {ACEESS_TOKEN}`


## How to deploy


## Future improvements
- Distributed cache
- Make image uploading async
- Improve cursor-based navigation
- Add CDN for images

