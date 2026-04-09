create proc [FrameworkCore].[OrganizationRoleInsert] (
     @SessionId uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@Name nvarchar(75)
    ,@Permissions Framework.IdList readonly
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

insert [Framework].[Role] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Name
    ,OrganizationId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Name
    ,@OrganizationId
)

declare @newRolePermissionIds Framework.NewIdList

insert @newRolePermissionIds
select Id, newid()
from @Permissions
where Id not in (
    select PermissionId
    from [Framework].[RolePermission-Active]
    where RoleId = @Id
)

insert [Framework].[RolePermission] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,RoleId
    ,PermissionId
)
select
     [NewId]
    ,[NewId]
    ,@now
    ,@SessionId
    ,@Id
    ,[Id]
from @newRolePermissionIds

commit transaction