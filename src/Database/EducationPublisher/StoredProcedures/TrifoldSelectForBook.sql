create proc [EducationPublisher].[TrifoldSelectForBook] (
     @SessionId uniqueidentifier
    ,@BookId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     trifold.Id
    ,trifold.BookId
    ,book.[Key] as BookKey
    ,trifold.Title
    ,trifold.StatusId
    ,status.Name as StatusName
    ,trifold.RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,trifold.GeneratesAchievementId
    ,generatesAchievement.Title as GeneratesAchievementTitle
    ,trifold.Ordinal
    ,binder.Id as BinderId
    ,binder.TypeId as BinderTypeId
    ,type.Name as BinderTypeName
from [Education].[Trifold-Active] trifold
    inner join [Content].[Binder-Active] binder on trifold.BinderId = binder.Id
    inner join [Education].[Book-Active] book on trifold.BookId = book.Id
    left join [Education].[Achievement-Active] generatesAchievement on trifold.GeneratesAchievementId = generatesAchievement.Id
    inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    left join [Education].[Achievement-Active] requiresAchievement on trifold.RequiresAchievementId = requiresAchievement.Id
    inner join [Framework].[ContentStatus-Active] status on trifold.StatusId = status.Id
    inner join [Content].[BinderType-Active] type on binder.TypeId = type.Id
where trifold.BookId = @BookId
    and organization.Id = @organizationId