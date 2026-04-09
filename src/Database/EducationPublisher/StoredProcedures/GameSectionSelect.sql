create proc [EducationPublisher].[GameSectionSelect] (
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
     gameSection.Id
    ,gameSection.GameId
    ,game.[Key] as GameKey
    ,gameSection.Title
    ,gameSection.StatusId
    ,status.Name as StatusName
    ,gameSection.TypeId
    ,type.Name as TypeName
    ,gameSection.Ordinal
    ,(select count(1) from [Education].[GameActivity-Active] where SectionId = gameSection.Id) as GameActivityCount
from [Education].[GameSection-Active] gameSection
    inner join [Education].[Game-Active] game on gameSection.GameId = game.Id
    inner join [Education].[Book-Active] book on game.BookId = book.Id
    inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    inner join [Framework].[ContentStatus-Active] status on gameSection.StatusId = status.Id
    inner join [Education].[GameSectionType-Active] type on gameSection.TypeId = type.Id
where gameSection.Id = @Id
    and organization.Id = @organizationId