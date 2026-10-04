create proc [ContentDesign].[PublicationGroupSelectOrderables] (
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
     publicationGroup.Id
    ,publicationGroup.Name as Name
    ,publicationGroup.Ordinal
from [Content].[PublicationGroup-Active] publicationGroup
    inner join [Framework].[Portal-Active] portal on publicationGroup.PortalId = portal.Id
where portal.OwnerId = @organizationId
order by publicationGroup.Ordinal