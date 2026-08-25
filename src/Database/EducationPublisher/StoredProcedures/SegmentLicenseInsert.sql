create proc [EducationPublisher].[SegmentLicenseInsert] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
    ,@SegmentId uniqueidentifier
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

if exists (
    select 1
    from [Framework].[SegmentLicense-Active] with (updlock, holdlock)
    where SegmentId = @SegmentId
        and LicenseId = @LicenseId
)
begin
    rollback transaction
    raiserror('An active relationship already exists for this license and content.', 16, 1)
    return
end

insert [Framework].[SegmentLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,LicenseId
    ,SegmentId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@LicenseId
    ,@SegmentId
)

if not exists (
    select 1
    from [Framework].[SegmentLicense-Active] segmentLicense
        inner join [Framework].[License-Active] license on segmentLicense.LicenseId = license.Id
        inner join [Framework].[Segment-Active] segment on segmentLicense.SegmentId = segment.Id
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    where segmentLicense.Id = @Id
        and portal.OwnerId = @organizationId
        and license.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction