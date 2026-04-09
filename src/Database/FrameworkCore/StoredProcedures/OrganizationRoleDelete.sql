create proc [FrameworkCore].[OrganizationRoleDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Framework].[Role]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where Id = @Id

update [Framework].[RolePermission]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where IsDeleted = 0
    and RoleId = @Id

commit transaction