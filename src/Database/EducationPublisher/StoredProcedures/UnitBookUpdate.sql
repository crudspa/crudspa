create proc [EducationPublisher].[UnitBookUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@BookId uniqueidentifier
    ,@Treatment bit
    ,@Control bit
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

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,BookId = @BookId
    ,Treatment = @Treatment
    ,Control = @Control
from [Education].[UnitBook] baseTable
    inner join [Education].[UnitBook-Active] unitBook on unitBook.Id = baseTable.Id
    inner join [Education].[Unit-Active] unit on unitBook.UnitId = unit.Id
    inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction