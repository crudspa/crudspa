create proc [ContentDesign].[BlogInsert] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@Title nvarchar(150)
    ,@StatusId uniqueidentifier
    ,@Author nvarchar(150)
    ,@Description nvarchar(max)
    ,@ImageId uniqueidentifier
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

insert [Content].[Blog] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PortalId
    ,Title
    ,StatusId
    ,Author
    ,Description
    ,ImageId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PortalId
    ,@Title
    ,@StatusId
    ,@Author
    ,@Description
    ,@ImageId
)

if not exists (
    select 1
    from [Content].[Blog-Active] blog
        inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where blog.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction