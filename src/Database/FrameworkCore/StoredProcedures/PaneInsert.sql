create proc [FrameworkCore].[PaneInsert] (
     @SessionId uniqueidentifier
    ,@SegmentId uniqueidentifier
    ,@Title nvarchar(150)
    ,@Key nvarchar(75)
    ,@TypeId uniqueidentifier
    ,@PermissionId uniqueidentifier
    ,@ConfigJson nvarchar(max)
    ,@Ordinal int
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

insert [Framework].[Pane] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,SegmentId
    ,Title
    ,[Key]
    ,TypeId
    ,PermissionId
    ,ConfigJson
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@SegmentId
    ,@Title
    ,@Key
    ,@TypeId
    ,@PermissionId
    ,@ConfigJson
    ,@Ordinal
)

if not exists (
    select 1
    from [Framework].[Pane-Active] pane
        inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where pane.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction