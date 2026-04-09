create proc [ContentDesign].[ForumDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @portalId uniqueidentifier = (select top 1 PortalId from [Content].[Forum] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Content].[Forum-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Content].[Forum-Active] forum
        inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where forum.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update forum
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Forum] forum
where forum.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Content].[Forum] baseTable
    inner join [Content].[Forum-Active] forum on forum.Id = baseTable.Id
where forum.PortalId = @portalId
    and forum.Ordinal > @oldOrdinal

commit transaction