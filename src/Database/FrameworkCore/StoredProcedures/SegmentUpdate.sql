create proc [FrameworkCore].[SegmentUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Key nvarchar(100)
    ,@StatusId uniqueidentifier
    ,@Title nvarchar(150)
    ,@PermissionId uniqueidentifier
    ,@IconId uniqueidentifier
    ,@Fixed bit
    ,@RequiresId bit
    ,@Recursive bit
    ,@AllLicenses bit
    ,@Licenses Framework.IdList readonly
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
    ,[Key] = @Key
    ,StatusId = @StatusId
    ,Title = @Title
    ,PermissionId = @PermissionId
    ,IconId = @IconId
    ,Fixed = @Fixed
    ,RequiresId = @RequiresId
    ,Recursive = @Recursive
    ,AllLicenses = @AllLicenses
from [Framework].[Segment] baseTable
    inner join [Framework].[Segment-Active] segment on segment.Id = baseTable.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end


update segmentLicense
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Framework].[SegmentLicense] segmentLicense
    left join @Licenses ids on ids.Id = segmentLicense.LicenseId
where segmentLicense.SegmentId = @Id
    and segmentLicense.IsDeleted = 0
    and ids.Id is null

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
    left join [Framework].[SegmentLicense-Active] existingJunction on existingJunction.SegmentId = @Id
        and existingJunction.LicenseId = ids.Id
    cross apply (select newid() as JunctionId) newRow
where existingJunction.Id is null
commit transaction