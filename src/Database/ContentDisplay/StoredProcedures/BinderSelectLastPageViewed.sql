create proc [ContentDisplay].[BinderSelectLastPageViewed] (
     @BinderId uniqueidentifier
    ,@SessionId uniqueidentifier
    ,@PageId uniqueidentifier output
) as

set @PageId = (
    select top 1 pageViewed.PageId
    from [Content].[PageViewed] pageViewed
        inner join [Content].[Page-Active] page on pageViewed.PageId = page.Id
        inner join [Content].[Binder-Active] binder on page.BinderId = binder.Id
    where binder.Id = @BinderId
        and pageViewed.UpdatedBy = @SessionId
    order by pageViewed.Updated desc
)