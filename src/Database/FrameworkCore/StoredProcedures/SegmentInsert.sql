create proc [FrameworkCore].[SegmentInsert] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@ParentId uniqueidentifier
    ,@Key nvarchar(100)
    ,@StatusId uniqueidentifier
    ,@Title nvarchar(150)
    ,@PermissionId uniqueidentifier
    ,@IconId uniqueidentifier
    ,@Fixed bit
    ,@RequiresId bit
    ,@Recursive bit
    ,@TypeId uniqueidentifier
    ,@AllLicenses bit
    ,@Licenses Framework.IdList readonly
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
declare @ordinal int

if (@ParentId is not null) begin
    set @ordinal = (select max(Ordinal) + 1 from [Framework].[Segment-Active] where ParentId = @ParentId)
    set @PortalId = (select top 1 PortalId from [Framework].[Segment-Active] where Id = @ParentId)
end
else begin
    set @ordinal = (select max(Ordinal) + 1 from [Framework].[Segment-Active] where PortalId = @PortalId)
end

if (@ordinal is null) set @ordinal = 0


insert [Framework].[Segment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PortalId
    ,ParentId
    ,[Key]
    ,StatusId
    ,Title
    ,PermissionId
    ,IconId
    ,Fixed
    ,RequiresId
    ,Recursive
    ,TypeId
    ,AllLicenses
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PortalId
    ,@ParentId
    ,@Key
    ,@StatusId
    ,@Title
    ,@PermissionId
    ,@IconId
    ,@Fixed
    ,@RequiresId
    ,@Recursive
    ,@TypeId
    ,@AllLicenses
    ,@ordinal
)

if not exists (
    select 1
    from [Framework].[Segment-Active] segment
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where segment.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

insert [Framework].[SegmentLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,SegmentId
    ,LicenseId
)
select
     newRow.JunctionId
    ,newRow.JunctionId
    ,@now
    ,@SessionId
    ,@Id
    ,ids.Id
from (select distinct Id from @Licenses) ids
    inner join [Framework].[License-Active] license on license.Id = ids.Id
    cross apply (select newid() as JunctionId) newRow

commit transaction