create proc [FrameworkCore].[UserInsert] (
     @SessionId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@Username nvarchar(150)
    ,@ResetPassword bit
    ,@Roles Framework.IdList readonly
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Framework].[User] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ContactId
    ,PortalId
    ,OrganizationId
    ,Username
    ,ResetPassword
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ContactId
    ,@PortalId
    ,@OrganizationId
    ,@Username
    ,@ResetPassword
)

insert [Framework].[UserRole] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,UserId
    ,RoleId
)
select
     new.UserRoleId
    ,new.UserRoleId
    ,@now
    ,@SessionId
    ,@Id
    ,roles.RoleId
from (select distinct Id as RoleId from @Roles) roles
    inner join [Framework].[Role-Active] role on role.Id = roles.RoleId
        and role.OrganizationId = @OrganizationId
    left join [Framework].[UserRole-Active] userRole on userRole.UserId = @Id
        and userRole.RoleId = roles.RoleId
    cross apply (select newid() as UserRoleId) new
where userRole.RoleId is null

commit transaction