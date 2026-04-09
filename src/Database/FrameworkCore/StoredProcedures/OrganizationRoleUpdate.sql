create proc [FrameworkCore].[OrganizationRoleUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Name nvarchar(75)
    ,@Permissions Framework.IdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Framework].[Role]
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,Name = @Name
where Id = @Id

declare @newRolePermissionIds Framework.NewIdList

update [Framework].[RolePermission]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where RoleId = @Id
    and IsDeleted = 0
    and PermissionId not in (select Id from @Permissions)

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