## Imagegram

API that allows you to upload posts with images and comment on them

### Tech stack
- .NET 6 
- SQL Server
- Azure Blob Storage

## Architecture decision records
- [Decision on choosing database SQL vs NoSQL](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20Database.SQL%20vs%20NoSQL.md)
- [Decision on sync vs async image uploading](https://github.com/shamil-sadigov/Imagegram/blob/master/docs/Decision%20on%20sync%20vs%20async%20image%20uploading.md)
- How cursor-based navigation work

## How cursor-based navigation work

When client should be able to retrieve posts via cursor-based pagination, and posts should be sorted by the number of comments (desc).


Let's say that we have 11 posts in DB.

```
ID      CommentCount    LastTimeUpdatedAt             Comments
----------------------------------------------------------------------
1       0               01.11.2022 5:15:52            []   
2       4               31.10.2022 18:23:51           [..., ...] 
3       8               30.10.2022 18:23:51           [..., ...]
4       4               31.10.2022 18:23:51           [..., ...]      
5       4               30.10.2022 12:23:51           [..., ...]      
6       9               30.10.2022 18:23:51           [..., ...]
7       8               01.11.2022 4:23:51            [..., ...]      
8       1               21.10.2022 18:23:51           [..., ...] 
9       0               01.11.2022 5:15:52            []      
10      5               31.10.2022 18:23:51           [..., ...]      
11      3               12.10.2022 12:12:51           [..., ...]    
```

Here are posts that are sorted by number of comments (desc)

```
ID      CommentCount    LastTimeUpdatedAt             Comments
----------------------------------------------------------------------
6       9               30.10.2022 18:23:51           [..., ...]
7       8               01.11.2022 4:23:51            [..., ...]      
3       8               30.10.2022 18:23:51           [..., ...]
10      5               31.10.2022 18:23:51           [..., ...]      
4       4               31.10.2022 18:23:51           [..., ...]      
2       4               31.10.2022 18:23:51           [..., ...] 
5       4               30.10.2022 12:23:51           [..., ...]      
11      3               12.10.2022 12:12:51           [..., ...]      
8       1               21.10.2022 18:23:51           [..., ...] 
9       0               01.11.2022 5:15:52            []      
1       0               01.11.2022 5:15:52            []   
```

So we need to choose approriate cursor to be able identify the row in database.

One option is to  create cursor based on (`CommentCount:LastTimeUpdatedAt`), but problem is that when client send us this cursor we could fail to identify which exactly row the cursor is pointing at, because there can be multiple rows with the same `CommentCount` and `LastTimeUpdatedAt` (for example post with IDs `4` and `2` has the same `CommentCount` and `LastTimeUpdatedAt`).

So, better option would be to create cursor base on (`CommentCount:LastTimeUpdatedAt:PostId`)





BUt this approach is still bad

It's good enough but not the best


## How to use API


## How to deploy


## Future improvements
- Distributed cache
- Make image uploading async
- Improve cursor-based navigation
- Add CDN for images

