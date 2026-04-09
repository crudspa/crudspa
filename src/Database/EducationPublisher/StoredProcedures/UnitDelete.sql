create proc [EducationPublisher].[UnitDelete] (
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

declare @oldOrdinal int = (
    select top 1 Ordinal
    from [Education].[Unit-Active] unit
    inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
    where unit.Id = @Id
    and organization.Id = @organizationId
)

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[Unit] baseTable
    inner join [Education].[Unit-Active] unit on unit.Id = baseTable.Id
    inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
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
from [Education].[Unit] baseTable
    inner join [Education].[Unit-Active] unit on unit.Id = baseTable.Id
where unit.Ordinal > @oldOrdinal

commit transaction