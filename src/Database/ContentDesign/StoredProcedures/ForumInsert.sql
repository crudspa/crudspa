create proc [ContentDesign].[ForumInsert] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@Title nvarchar(150)
    ,@StatusId uniqueidentifier
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

declare @ordinal int = (select count(1) from [Content].[Forum-Active] where PortalId = @PortalId)

insert [Content].[Forum] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PortalId
    ,Title
    ,StatusId
    ,Description
    ,ImageId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PortalId
    ,@Title
    ,@StatusId
    ,@Description
    ,@ImageId
    ,@ordinal
)

if not exists (
    select 1
    from [Content].[Forum-Active] forum
        inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where forum.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction