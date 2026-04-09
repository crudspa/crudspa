create proc [EducationPublisher].[UnitBookDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @unitId uniqueidentifier = (select top 1 UnitId from [Education].[UnitBook] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[UnitBook-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Education].[UnitBook-Active] unitBook
        inner join [Education].[Unit-Active] unit on unitBook.UnitId = unit.Id
        inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
    where unitBook.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update unitBook
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[UnitBook] unitBook
where unitBook.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[UnitBook] baseTable
    inner join [Education].[UnitBook-Active] unitBook on unitBook.Id = baseTable.Id
where unitBook.UnitId = @unitId
    and unitBook.Ordinal > @oldOrdinal

commit transaction