create proc [ContentDesign].[PublicationDelete] (
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
    from [Content].[Publication-Active] publication
    inner join [Framework].[Portal-Active] portal on publication.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where publication.Id = @Id
    and organization.Id = @organizationId
)

declare @oldOrdinal int = (
    select top 1 Ordinal
    from [Content].[Publication-Active] publication
    inner join [Framework].[Portal-Active] portal on publication.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where publication.Id = @Id
    and organization.Id = @organizationId
)

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Publication] baseTable
    inner join [Content].[Publication-Active] publication on publication.Id = baseTable.Id
    inner join [Framework].[Portal-Active] portal on publication.PortalId = portal.Id
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
from [Content].[Publication] baseTable
    inner join [Content].[Publication-Active] publication on publication.Id = baseTable.Id
where publication.PortalId = @portalId
    and publication.Ordinal > @oldOrdinal

commit transaction