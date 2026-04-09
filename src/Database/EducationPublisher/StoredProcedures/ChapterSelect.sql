create proc [EducationPublisher].[ChapterSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     chapter.Id
    ,chapter.BookId
    ,book.[Key] as BookKey
    ,chapter.Title
    ,chapter.Ordinal
    ,binder.Id as BinderId
    ,binder.TypeId as BinderTypeId
    ,type.Name as BinderTypeName
    ,(select count(1) from [Content].[Page-Active] where BinderId = chapter.BinderId) as PageCount
from [Education].[Chapter-Active] chapter
    inner join [Content].[Binder-Active] binder on chapter.BinderId = binder.Id
    inner join [Education].[Book-Active] book on chapter.BookId = book.Id
    inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    inner join [Content].[BinderType-Active] type on binder.TypeId = type.Id
where chapter.Id = @Id
    and organization.Id = @organizationId