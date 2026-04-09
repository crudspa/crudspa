create proc [EducationPublisher].[GameActivitySelect] (
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
     gameActivity.Id
    ,gameActivity.SectionId
    ,section.Title as SectionTitle
    ,gameActivity.ThemeWord
    ,gameActivity.GroupId
    ,groupTable.Name as GroupName
    ,gameActivity.TypeId
    ,type.Name as TypeName
    ,gameActivity.Rigorous
    ,gameActivity.Multisyllabic
    ,gameActivity.ActivityId
    ,activity.[Key] as ActivityKey
    ,gameActivity.Ordinal
from [Education].[GameActivity-Active] gameActivity
    inner join [Education].[Activity-Active] activity on gameActivity.ActivityId = activity.Id
    inner join [Education].[GameSection-Active] gameSection on gameActivity.SectionId = gameSection.Id
    inner join [Education].[Game-Active] game on gameSection.GameId = game.Id
    inner join [Education].[Book-Active] book on game.BookId = book.Id
    left join [Education].[ResearchGroup-Active] groupTable on gameActivity.GroupId = groupTable.Id
    inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    inner join [Education].[GameSection-Active] section on gameActivity.SectionId = section.Id
    inner join [Education].[GameActivityType-Active] type on gameActivity.TypeId = type.Id
where gameActivity.Id = @Id
    and organization.Id = @organizationId

select
    activity.*
from [Education].[GameActivity-Active] gameActivity
    inner join [Education].[Activity-Deep] activity on gameActivity.ActivityId = activity.Id
where gameActivity.Id = @Id

select
    activityChoice.*
from [Education].[ActivityChoice-Deep] activityChoice
    inner join [Education].[Activity-Active] activity on activityChoice.ActivityId = activity.Id
    inner join [Education].[GameActivity-Active] gameActivity on gameActivity.ActivityId = activity.Id
where gameActivity.Id = @Id