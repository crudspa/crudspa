create proc [EducationPublisher].[ForumSelectOrderables] (
     @SessionId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     forum.Id
    ,forum.Title as Name
    ,forum.Ordinal
from [Content].[Forum-Active] forum
    inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
where portal.OwnerId = @organizationId
order by forum.Ordinal