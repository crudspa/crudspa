create proc [FrameworkCore].[SegmentSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @PaneTypeSinglePane uniqueidentifier = '35f404f9-c08b-4c71-88c9-794b60741332'

set nocount on

select
     segment.Id
    ,segment.PortalId
    ,segment.Title
    ,segment.[Key]
    ,segment.StatusId
    ,status.Name as StatusName
    ,segment.PermissionId
    ,permission.Name as PermissionName
    ,segment.IconId
    ,icon.CssClass as IconCssClass
    ,segment.Fixed
    ,segment.RequiresId
    ,segment.Recursive
    ,segment.TypeId
    ,type.Name as TypeName
    ,segment.AllLicenses
    ,segment.ParentId
    ,segment.Ordinal
    ,(select count(1) from [Framework].[Segment-Active] where ParentId = segment.Id) as SegmentCount
    ,case
        when segment.TypeId = @PaneTypeSinglePane then 1
        else (
            select count(1)
            from [Framework].[Pane-Active] pane
                inner join [Framework].[PaneType-Active] paneType on pane.TypeId = paneType.Id
            where pane.SegmentId = segment.Id
        )
    end as PaneCount
from [Framework].[Segment-Active] segment
    left join [Framework].[Icon-Active] icon on segment.IconId = icon.Id
    left join [Framework].[Segment-Active] parent on segment.ParentId = parent.Id
    left join [Framework].[Permission-Active] permission on segment.PermissionId = permission.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[ContentStatus-Active] status on segment.StatusId = status.Id
    inner join [Framework].[SegmentType-Active] type on segment.TypeId = type.Id
where segment.Id = @Id
    and organization.Id = @organizationId

select distinct
     @Id as SegmentId
    ,license.Id as LicenseId
    ,license.Name as LicenseName
    ,convert(bit, iif(segmentLicense.Id is null, 0, 1)) as Selected
from [Framework].[License-Active] license
    left join [Framework].[SegmentLicense-Active] segmentLicense on segmentLicense.LicenseId = license.Id
        and segmentLicense.SegmentId = @Id
where license.OwnerId = @organizationId
order by license.Name