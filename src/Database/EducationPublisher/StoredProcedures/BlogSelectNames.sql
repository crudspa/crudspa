create proc [EducationPublisher].[BlogSelectNames] (
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
     blog.Id
    ,blog.Title as Name
from [Content].[Blog-Active] blog
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
where portal.OwnerId = @organizationId
order by blog.Title