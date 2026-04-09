create proc [EducationPublisher].[ModuleSelectForBook] (
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
     module.Id
    ,module.BookId
    ,book.[Key] as BookKey
    ,module.Title
    ,module.StatusId
    ,status.Name as StatusName
    ,module.IconId
    ,icon.CssClass as IconCssClass
    ,module.RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,module.GeneratesAchievementId
    ,generatesAchievement.Title as GeneratesAchievementTitle
    ,module.Ordinal
    ,binder.Id as BinderId
    ,binder.TypeId as BinderTypeId
    ,type.Name as BinderTypeName
from [Education].[Module-Active] module
    inner join [Content].[Binder-Active] binder on module.BinderId = binder.Id
    inner join [Education].[Book-Active] book on module.BookId = book.Id
    left join [Education].[Achievement-Active] generatesAchievement on module.GeneratesAchievementId = generatesAchievement.Id
    left join [Framework].[Icon-Active] icon on module.IconId = icon.Id
    inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    left join [Education].[Achievement-Active] requiresAchievement on module.RequiresAchievementId = requiresAchievement.Id
    inner join [Framework].[ContentStatus-Active] status on module.StatusId = status.Id
    inner join [Content].[BinderType-Active] type on binder.TypeId = type.Id
where module.BookId = @BookId
    and organization.Id = @organizationId