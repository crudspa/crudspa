create proc [EducationPublisher].[SegmentLicenseUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@SegmentId uniqueidentifier
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

if exists (
    select 1
    from [Framework].[SegmentLicense-Active] with (updlock, holdlock)
    where Id != @Id
        and SegmentId = @SegmentId
        and LicenseId = (select LicenseId from [Framework].[SegmentLicense-Active] where Id = @Id)
)
begin
    rollback transaction
    raiserror('An active relationship already exists for this license and content.', 16, 1)
    return
end

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,SegmentId = @SegmentId
from [Framework].[SegmentLicense] baseTable
    inner join [Framework].[SegmentLicense-Active] segmentLicense on segmentLicense.Id = baseTable.Id
    inner join [Framework].[License-Active] license on segmentLicense.LicenseId = license.Id
    inner join [Framework].[Segment-Active] segment on segmentLicense.SegmentId = segment.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
where baseTable.Id = @Id
    and portal.OwnerId = @organizationId
    and license.OwnerId = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction