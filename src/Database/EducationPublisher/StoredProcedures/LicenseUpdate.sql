create proc [EducationPublisher].[LicenseUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Name nvarchar(50)
    ,@Description nvarchar(max)
    ,@Segments Framework.IdList readonly
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
    ,Name = @Name
    ,Description = @Description
from [Framework].[License] baseTable
    inner join [Framework].[License-Active] license on license.Id = baseTable.Id
    inner join [Framework].[Organization-Active] organization on license.OwnerId = organization.Id
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
    inner join [Framework].[Segment-Active] segment on segment.Id = segmentLicense.SegmentId
    inner join [Framework].[Portal-Active] portal on portal.Id = segment.PortalId
        and portal.OwnerId = @organizationId
    left join @Segments ids on ids.Id = segmentLicense.SegmentId
where segmentLicense.LicenseId = @Id
    and segmentLicense.IsDeleted = 0
    and ids.Id is null

insert [Framework].[SegmentLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,LicenseId
    ,SegmentId
)
select
     newRow.JunctionId
    ,newRow.JunctionId
    ,@now
    ,@SessionId
    ,@Id
    ,ids.Id
from (select distinct Id from @Segments) ids
    inner join [Framework].[Segment-Active] segment on segment.Id = ids.Id
    inner join [Framework].[Portal-Active] portal on portal.Id = segment.PortalId
        and portal.OwnerId = @organizationId
    left join [Framework].[SegmentLicense-Active] existingJunction on existingJunction.LicenseId = @Id
        and existingJunction.SegmentId = ids.Id
    cross apply (select newid() as JunctionId) newRow
where existingJunction.Id is null
commit transaction