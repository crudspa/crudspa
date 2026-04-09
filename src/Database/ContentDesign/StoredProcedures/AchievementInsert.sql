create proc [ContentDesign].[AchievementInsert] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@Title nvarchar(75)
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

insert [Content].[Achievement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PortalId
    ,Title
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
    ,@Description
    ,@ImageId
)

if not exists (
    select 1
    from [Content].[Achievement-Active] achievement
        inner join [Framework].[Portal-Active] portal on achievement.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where achievement.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction