create proc [EducationPublisher].[LicenseInsert] (
     @SessionId uniqueidentifier
    ,@Name nvarchar(50)
    ,@Description nvarchar(max)
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

insert [Framework].[License] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Name
    ,Description
    ,OwnerId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Name
    ,@Description
    ,@organizationId
)

if not exists (
    select 1
    from [Framework].[License-Active] license
        inner join [Framework].[Organization-Active] organization on license.OwnerId = organization.Id
    where license.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction