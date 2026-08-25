create proc [EducationStudent].[ChapterSelectForBook] (
     @BookId uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

insert [Education].[ContentViewed] (
    Id
    ,Updated
    ,UpdatedBy
    ,BookId
)
select
    newid()
    ,@now
    ,@SessionId
    ,@BookId
where exists (
    select 1
    from [EducationStudent].[LicensedBooks](@SessionId, @BookId) licensedBook
)

select
     chapter.Id
    ,chapter.Title
    ,chapter.BookId
    ,chapter.BinderId
    ,chapter.Ordinal
    ,(select count(1) from [Content].[Page-Active] page where page.BinderId = chapter.BinderId and page.StatusId = @ContentStatusComplete) as PageCount
    ,binderType.DisplayView as BinderDisplayView
from [Education].[Chapter-Active] chapter
    inner join [Content].[Binder-Active] binder on chapter.BinderId = binder.Id
    inner join [Content].[BinderType-Active] binderType on binder.TypeId = binderType.Id
where chapter.BookId = @BookId
    and exists (
        select 1
        from [EducationStudent].[LicensedBooks](@SessionId, chapter.BookId) licensedBook
    )
    and (select count(1) from [Content].[Page-Active] page where page.BinderId = chapter.BinderId and page.StatusId = @ContentStatusComplete) > 0
order by chapter.Ordinal asc