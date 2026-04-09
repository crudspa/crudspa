create proc [ContentDesign].[BlogUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(150)
    ,@StatusId uniqueidentifier
    ,@Author nvarchar(150)
    ,@Description nvarchar(max)
    ,@ImageId uniqueidentifier
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
    ,Title = @Title
    ,StatusId = @StatusId
    ,Author = @Author
    ,Description = @Description
    ,ImageId = @ImageId
from [Content].[Blog] baseTable
    inner join [Content].[Blog-Active] blog on blog.Id = baseTable.Id
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction