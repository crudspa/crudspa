create proc [ContentDesign].[PublicationGroupDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @portalId uniqueidentifier = (
    select top 1 PortalId
    from [Content].[PublicationGroup-Active] publicationGroup
    inner join [Framework].[Portal-Active] portal on publicationGroup.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where publicationGroup.Id = @Id
    and organization.Id = @organizationId
)

declare @oldOrdinal int = (
    select top 1 Ordinal
    from [Content].[PublicationGroup-Active] publicationGroup
    inner join [Framework].[Portal-Active] portal on publicationGroup.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where publicationGroup.Id = @Id
    and organization.Id = @organizationId
)

update publication
set GroupId = null, Updated = @now, UpdatedBy = @SessionId
from Content.[Publication] publication
    inner join Content.[PublicationGroup-Active] groups on groups.Id = publication.GroupId
    inner join Framework.[Portal-Active] ownerPortal on ownerPortal.Id = groups.PortalId
where groups.Id = @Id and ownerPortal.OwnerId = @organizationId and publication.Id = publication.VersionOf and publication.IsDeleted = 0

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[PublicationGroup] baseTable
    inner join [Content].[PublicationGroup-Active] publicationGroup on publicationGroup.Id = baseTable.Id
    inner join [Framework].[Portal-Active] portal on publicationGroup.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Content].[PublicationGroup] baseTable
    inner join [Content].[PublicationGroup-Active] publicationGroup on publicationGroup.Id = baseTable.Id
where publicationGroup.PortalId = @portalId
    and publicationGroup.Ordinal > @oldOrdinal

commit transaction