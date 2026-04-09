create proc [EducationPublisher].[UnitBookInsert] (
     @SessionId uniqueidentifier
    ,@UnitId uniqueidentifier
    ,@BookId uniqueidentifier
    ,@Treatment bit
    ,@Control bit
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @ordinal int = (select count(1) from [Education].[UnitBook-Active] where UnitId = @UnitId)

insert [Education].[UnitBook] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,UnitId
    ,BookId
    ,Treatment
    ,Control
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@UnitId
    ,@BookId
    ,@Treatment
    ,@Control
    ,@ordinal
)

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

commit transaction