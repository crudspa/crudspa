create proc [ContentDesign].[TrackDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @portalId uniqueidentifier = (select top 1 PortalId from [Content].[Track] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Content].[Track-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Content].[Track-Active] track
        inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where track.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update track
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Track] track
where track.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Content].[Track] baseTable
    inner join [Content].[Track-Active] track on track.Id = baseTable.Id
where track.PortalId = @portalId
    and track.Ordinal > @oldOrdinal

commit transaction