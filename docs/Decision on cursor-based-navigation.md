# How cursor-based navigation implemented

Client should be able to retrieve posts via cursor-based pagination, and posts should be sorted by the number of comments (desc).

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

Here are posts that are sorted by number of comments (desc).
Actually it's also sorted by `LastTimeUpdatedAt` (desc).

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

One option is to  create cursor based on (`[CommentCount]:[LastTimeUpdatedAt]`), but problem is that when client send us this cursor we could fail to identify which exactly row the cursor is pointing at, because there can be multiple rows with the same `CommentCount` and `LastTimeUpdatedAt` (for example post with IDs `4` and `2` has the same `CommentCount` and `LastTimeUpdatedAt`).

So, better option would be to create cursor base on (`[CommentCount]:[LastTimeUpdatedAt]:[PostId]`)


### How it works

Let's say that client want to show 3 posts per page.

```
ID      CommentCount    LastTimeUpdatedAt             Comments
----------------------------------------------------------------------
6       9               30.10.2022 18:23:51           [..., ...]          <- Start cursor
7       8               01.11.2022 4:23:51            [..., ...]      
3       8               30.10.2022 18:23:51           [..., ...]          <- End cursor
---------------------------------------------------------------------
10      5               31.10.2022 18:23:51           [..., ...]          
4       4               31.10.2022 18:23:51           [..., ...]      
2       4               31.10.2022 18:23:51           [..., ...] 
-----------------------------------------------------------------------
5       4               30.10.2022 12:23:51           [..., ...]      
11      3               12.10.2022 12:12:51           [..., ...]      
8       1               21.10.2022 18:23:51           [..., ...] 
---------------------------------------------------------------------
9       0               01.11.2022 5:15:52            []      
1       0               01.11.2022 5:15:52            []   
```

This is how flow looks like

---

Get first page


```
GET ...api/posts?limit=3


RESPONSE 
{
  "requestedPageSize": 3,
  "actualPageSize": 3,
  "hasMoreItems": true,
  "items": [...posts...],
  "cursors": {
    "startCursor": "9:63802751031:6",
    "endCursor": "8:63802873431:3"
  }
}

```

As you see `hasMoreItems = true` which means that there are more posts available, so we can navigate to the next page.

---

Get second page, by specifying value of `endCursor` from the last response in `after` parameter

```
GET ...api/posts?limit=3&after=8:63802873431:3


RESPONSE 
{
  "requestedPageSize": 3,
  "actualPageSize": 3,
  "hasMoreItems": true,
  "items": [...posts...],
  "cursors": {
    "startCursor": "5:63802837431:10",
    "endCursor": "4:63802837431:2"
  }
}

```

`hasMoreItems = true` so there are more items available!

---

Get third page, by specifying value of `endCursor` from the last response

```
GET ...api/posts?limit=3&after=4:63802837431:2


RESPONSE 
{
  "requestedPageSize": 3,
  "actualPageSize": 3,
  "hasMoreItems": true,
  "items": [...posts...],
  "cursors": {
    "startCursor": "4:63802729431:5",
    "endCursor": "1:63801973431:8"
  }
}

```

`hasMoreItems = true` so there are more items available!

---

Get forth page, by specifying value of `endCursor` from the last response

```
GET ...api/posts?limit=3&after=1:63801973431:8


RESPONSE 
{
  "requestedPageSize": 3,
  "actualPageSize": 2,
  "hasMoreItems": false,
  "items": [...posts...],
  "cursors": {
    "startCursor": "0:63802729431:9",
    "endCursor": "0:63801973431:1"
  }
}

```

As you see, `hasMoreItems = false` which means that there is not items, and we cannot navigate to the next page. But we can navigate to previous page!

---

Go back to previous page (third), by specifying value of `startCursor` from the last response in `before` parameter

```
GET ...api/posts?limit=3&before=0:63802729431:9


RESPONSE 
{
  "requestedPageSize": 3,
  "actualPageSize": 3,
  "hasMoreItems": true,
  "items": [...posts...],
  "cursors": {
    "startCursor": "4:63802729431:5",
    "endCursor": "1:63801973431:8"
  }
}

```

So, probably you got the point how it works.
PS: Cursors are url encoded before they are sent to client, so they can look a little different than examples above.

---


## Problem

The problem is that approach to pagination outlined above has drawbacks, because it doesn't consider dynamic nature of `CommentCount` and `LastUpdatedAt` attributes.

While client is navigating throug pages, number of comments on some posts can be increased or decreased, so client probably will not see some of the posts, or may see duplicated posts. 

One of the solutions is to implement tricky [continuation token](https://phauer.com/2017/web-api-pagination-continuation-token/).

### Decision

I decided to stick with the solution described above, yep, it has drawbacks, it's not perfect, BUT it's less labor-intensive and just 'good enough' for the first release.
