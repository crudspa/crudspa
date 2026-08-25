create proc [EducationStudent].[ChapterSelect] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

begin transaction
    declare @ChapterViewedId uniqueidentifier
    set @ChapterViewedId = newid()
    insert [Education].[ChapterViewed] (
        Id
        ,Updated
        ,UpdatedBy
        ,ChapterId
    )
    select
        @ChapterViewedId
        ,@now
        ,@SessionId
        ,@Id
    where exists (
        select 1
        from [Education].[Chapter-Active] chapter
            inner join [EducationStudent].[LicensedBooks](@SessionId, null) licensedBook on licensedBook.BookId = chapter.BookId
        where chapter.Id = @Id
    )
commit transaction

select
    chapter.Id
    ,chapter.Title
    ,chapter.BookId
    ,chapter.BinderId
    ,chapter.Ordinal
    ,binderType.DisplayView as BinderDisplayView
from [Education].[Chapter-Active] chapter
    inner join [Content].[Binder-Active] binder on chapter.BinderId = binder.Id
    inner join [Content].[BinderType-Active] binderType on binder.TypeId = binderType.Id
where chapter.Id = @Id
    and exists (
        select 1
        from [EducationStudent].[LicensedBooks](@SessionId, chapter.BookId) licensedBook
    )