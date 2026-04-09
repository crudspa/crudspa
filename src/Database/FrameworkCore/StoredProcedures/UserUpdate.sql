create proc [FrameworkCore].[UserUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Username nvarchar(150)
    ,@ResetPassword bit
    ,@Roles Framework.IdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @organizationId uniqueidentifier = (select top 1 OrganizationId from [Framework].[User-Active] userTable where Id = @Id)

update userTable
set  Updated = @now
    ,UpdatedBy = @SessionId
    ,Username = @Username
    ,ResetPassword = @ResetPassword
from [Framework].[User] userTable
where userTable.Id = @Id

update userRole
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Framework].[UserRole] userRole
    left join @Roles roles on roles.Id = userRole.RoleId
where userRole.UserId = @Id
    and userRole.IsDeleted = 0
    and roles.Id is null

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
        and role.OrganizationId = @organizationId
    left join [Framework].[UserRole-Active] userRole on userRole.UserId = @Id
        and userRole.RoleId = roles.RoleId
    cross apply (select newid() as UserRoleId) new
where userRole.RoleId is null

commit transaction