create proc [EducationStudent].[ChapterSelect] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

begin transaction
    declare @ChapterViewedId uniqueidentifier
    set @ChapterViewedId = newid()
    insert [Education].[ChapterViewed] (
        Id
        ,Updated
        ,UpdatedBy
        ,ChapterId
    )
    values (
        @ChapterViewedId
        ,@now
        ,@SessionId
        ,@Id
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