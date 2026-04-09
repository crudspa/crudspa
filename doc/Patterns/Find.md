# Patterns | Find

`Find` is the discovery pattern. Use it when users need to search across a large or scoped dataset, page through results, and open focused panes from those matches.

It works well because search state stays typed, query execution stays server-side, and result freshness is handled through events instead of ad hoc UI logic.

## Canonical Terms

| Term | Meaning |
| --- | --- |
| Node | The service boundary that owns the search DTO, query methods, and result shape for one root entity |
| Predicate | An explicit query constraint used for scope, tenancy, and user-entered filtering |

## Search State

`Find` starts with explicit, typed search state.

```csharp
public class Search : Observable, IDisposable
{
    private void HandlePagedChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Paged));
    private void HandleSortChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Sort));

    private Paged _paged;
    private Sort _sort;

    public Search()
    {
        _paged = new();
        _paged.PropertyChanged += HandlePagedChanged;

        _sort = new();
        _sort.PropertyChanged += HandleSortChanged;
    }

    public virtual void Dispose()
    {
        _paged.PropertyChanged -= HandlePagedChanged;
        _sort.PropertyChanged -= HandleSortChanged;
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Paged Paged
    {
        get => _paged;
        set => SetProperty(ref _paged, value);
    }

    public Sort Sort
    {
        get => _sort;
        set => SetProperty(ref _sort, value);
    }

    public String? TimeZoneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ParentId
    {
        get;
        set => SetProperty(ref field, value);
    }
}
```

## Flow

1. The pane updates a `Search` object with text, paging, sorting, and any pattern-specific filters.
2. `FindModel<TSearch, TEntity>` refreshes when that state changes.
3. The client proxy sends `Request<TSearch>` through the hub.
4. The hub applies permission wrappers and enriches request context when needed.
5. The service executes a `SelectWhere` query and returns rows plus total count.
6. The model updates cards and pager state.
7. Save and remove events from other panes trigger refreshes so results stay current.

## Example: Post Find For Blog

A concrete find model sets deterministic defaults in `Reset`.

```csharp
public async Task Reset()
{
    _resetting = true;

    Search.ParentId = _blogId;

    Search.Text = String.Empty;

    Search.Paged.PageNumber = 1;
    Search.Paged.PageSize = 50;
    Search.Paged.TotalCount = 0;

    Search.Sort.Field = Sorts.First();
    Search.Sort.Ascending = true;
    Search.Status.Clear();
    Search.PublishedRange.Type = DateRange.Types.Any;
    Search.RevisedRange.Type = DateRange.Types.Any;

    await WithMany("Initializing...",
        FetchContentStatusNames());

    _resetting = false;

    await Refresh(false);
}
```

The hub can enrich search context before service execution.

```csharp
public async Task<Response<IList<Post>>> PostSearchForBlog(Request<PostSearch> request)
{
    return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
    {
        request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
        return await PostService.SearchForBlog(request);
    });
}
```

SQL keeps scope, filters, paging, and counts in one query.

```sql
;with PostCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Published' and @SortAscending = 1)
                    then post.Published
                end asc,
                case when (@SortField = 'Published' and @SortAscending = 0)
                    then post.Published
                end desc,
                case when (@SortAscending = 1)
                    then post.Id
                end asc,
                case when (@SortAscending = 0)
                    then post.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,post.Id
    from [Content].[Post-Active] post
        inner join [Content].[Blog-Active] blog on post.BlogId = blog.Id
        inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where post.BlogId = @BlogId
        and organization.Id = @organizationId
        and (@SearchText is null
            or post.Title like '%' + @SearchText + '%'
            or post.Author like '%' + @SearchText + '%'
        )
        and (@statusCount = 0 or post.StatusId in (select Id from @Status))
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,post.Id
    ,post.Title
    ,(select count(1) from [Content].[Comment-Active] where PostId = post.Id) as CommentCount
    ,(select count(1) from [Content].[Section-Active] where PageId = post.PageId) as SectionCount
from PostCte cte
    inner join [Content].[Post-Active] post on cte.Id = post.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
```

## Pressure Points

### Predicate Layering

Most real find queries combine several predicate classes at once:

* permission checks at the hub boundary
* parent scope such as `post.BlogId = @BlogId`
* tenancy checks such as `organization.Id = @organizationId`
* user-entered filters such as text, statuses, and date ranges

### Stable Sorting

Sorting must be deterministic. The `PostSelectWhereForBlog` query includes `post.Id` as a tie-break so paging does not reshuffle rows between requests.

### High-Cardinality Filters

Use table-valued parameters such as `Framework.IdList` for multi-select filters instead of parsing strings in SQL.

### Real-Time Drift

Search results become stale quickly in multi-user systems. `PostFindForBlogModel` refreshes when post and section events arrive so counts and result cards stay honest.

```csharp
public async Task Handle(PostAdded payload) => await Refresh();
public async Task Handle(PostSaved payload) => await Refresh();
public async Task Handle(PostRemoved payload) => await Refresh();
public async Task Handle(SectionAdded payload) => await Refresh();
public async Task Handle(SectionSaved payload) => await Refresh();
public async Task Handle(SectionRemoved payload) => await Refresh();
```

## Guidance

* Keep search DTOs explicit and avoid untyped filter bags.
* Let SQL own paging windows, total counts, and heavy query logic.
* Keep `Reset` defaults obvious and deterministic.
* Treat predicate logic as security-critical infrastructure.

A good `Find` flow keeps its query shape explicit all the way down: filter controls update typed search state, the request goes to the server, SQL owns predicates and paging, and the UI renders the resulting cards or rows. Filters should not dissolve into an untyped bag once they leave the client.

## Tradeoffs

`Find` gives powerful discovery, but it adds query-state complexity and lookup overhead. If the collection is small and mostly static, `List` is often cheaper.

## Next Steps

* [Patterns | List](List.md)
* [Patterns | Edit](Edit.md)
* [Documentation Index](../ReadMe.md)
