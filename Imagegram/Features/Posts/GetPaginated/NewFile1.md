limit - Same as before, amount of rows we want to show on one page
cursor - ID of a reference element in the list. This can be the first item if you're querying the previous page and the last item if querying the next page.
cursorDirection - If user clicked Next or Previous (after or before)

When requesting the first page, we don't need to provide anything, just the limit 10, saying how many rows we want to get. As a result, we get our ten rows.

----

NOT THAT BAD

type PageInfo {
  hasNextPage: Boolean!
  hasPreviousPage: Boolean!
  startCursor: ID!
  endCursor: ID!
}

----------------

THIS is how it's gonna work

TODO: ADD support for `HasPreviousPage` and `HasNextPage`


### Initial request

```
GET api/posts?size=10

RESPONSE
{
    Size = 10
    Items = [...]
    StartCursor = 1
    EndCursor = 10,
    HasMore = true
}
```




### Navigate to next page

```
GET api/posts?size=10&after={EndCursor = 10}

RESPONSE
{
    Size = 10
    Items = [...]
    StartCursor = 11
    EndCursor = 21,
    HasMore = true
}
```

### Navigate to previous next page

```
GET api/posts?size=10&before={StartCursor = 11}

RESPONSE
{
    Size = 10
    Items = [...]
    StartCursor = 1
    EndCursor = 10,
    HasMore = true
}
```


NEW IDEA

{
"links": {
"prev": "/example-data?page[before]=yyy&page[size]=2",
"next": "/example-data?page[after]=zzz&page[size]=2"
},
"data": [
// the pagination item metadata is optional below.
{ "type": "examples", "id": "7", "meta": { "page": { "cursor": "yyy" } } },
{ "type": "examples", "id": "8", "meta": { "page": { "cursor": "zzz" } }  }
]
}