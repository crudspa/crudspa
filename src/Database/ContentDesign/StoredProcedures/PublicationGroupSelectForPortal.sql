create proc [ContentDesign].[PublicationGroupSelectForPortal] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
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
    ,publicationGroup.PortalId
    ,publicationGroup.Name
    ,publicationGroup.Ordinal
from [Content].[PublicationGroup-Active] publicationGroup
    inner join [Framework].[Portal-Active] portal on publicationGroup.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where publicationGroup.PortalId = @PortalId
    and organization.Id = @organizationId