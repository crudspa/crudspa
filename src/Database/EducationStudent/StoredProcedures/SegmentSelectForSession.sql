create proc [EducationStudent].[SegmentSelectForSession] (
     @SessionId uniqueidentifier,
    @PortalId uniqueidentifier
) as
begin
    set nocount on;

    declare @contentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c';
    declare @sessionUserId uniqueidentifier = null;

    select
        @sessionUserId = session.UserId
    from [Framework].[Session-Active] session
        left join [Framework].[User-Active] userTable on userTable.Id = session.UserId
    where session.Id = @SessionId
        and session.PortalId = @PortalId
        and session.Ended is null;

    create table #sessionPermissions (
        PermissionId uniqueidentifier not null primary key
    );

    insert into #sessionPermissions (PermissionId)
    select distinct rolePermission.PermissionId
    from [Framework].[UserRole-Active] userRole
        inner join [Framework].[RolePermission-Active] rolePermission on rolePermission.RoleId = userRole.RoleId
        inner join [Framework].[PortalPermission-Active] portalPermission on portalPermission.PermissionId = rolePermission.PermissionId
    where userRole.UserId = @sessionUserId
        and portalPermission.PortalId = @PortalId;

    create table #sessionLicenses (
        LicenseId uniqueidentifier not null primary key
    );

    insert into #sessionLicenses (LicenseId)
    select sessionLicense.LicenseId
    from [EducationCommon].[SessionLicenses](@SessionId) sessionLicense;

    create table #licensedSegments (
        Id uniqueidentifier not null primary key
    );

    insert into #licensedSegments (Id)
    select distinct segmentLicense.SegmentId
    from [Framework].[SegmentLicense-Active] segmentLicense
        inner join #sessionLicenses sessionLicense on sessionLicense.LicenseId = segmentLicense.LicenseId;

    create table #segmentsTable (
        Id uniqueidentifier not null primary key
    );

    insert into #segmentsTable (Id)
    select segment.Id
    from [Framework].[Segment-Active] segment
    where segment.StatusId = @contentStatusComplete
        and segment.PortalId = @PortalId
        and (
            segment.PermissionId is null
            or exists (
                select 1
                from #sessionPermissions permission
                where permission.PermissionId = segment.PermissionId
            )
        )
        and (
            segment.AllLicenses = 1
            or exists (
                select 1
                from #licensedSegments licensedSegment
                where licensedSegment.Id = segment.Id
            )
        );

    select
        segment.Id,
        segment.StatusId,
        segment.PortalId,
        segment.PermissionId,
        segment.ParentId,
        segment.[Key],
        segment.Title,
        segment.Fixed,
        segment.Routable,
        segment.Navigable,
        segment.Mapable,
        segment.RequiresId,
        segment.TypeId,
        segment.IconId,
        segment.Recursive,
        segment.Ordinal,
        type.DisplayView as TypeDisplayView,
        icon.CssClass as IconCssClass
    from #segmentsTable cte
        inner join [Framework].[Segment-Active] segment on segment.Id = cte.Id
        inner join [Framework].[SegmentType-Active] type on segment.TypeId = type.Id
        left join [Framework].[Icon-Active] icon on segment.IconId = icon.Id
    order by
        segment.Ordinal,
        segment.Id;

    select
        pane.Id,
        pane.[Key],
        pane.Title,
        pane.SegmentId,
        pane.TypeId,
        pane.ConfigJson,
        pane.Ordinal,
        type.DisplayView as TypeDisplayView,
        pagePane.PageId,
        binderPane.BinderId,
        coursePane.CourseId,
        coursePane.IdSource
    from [Framework].[Pane-Active] pane
        inner join [Framework].[PaneType-Active] type on pane.TypeId = type.Id
        inner join [Framework].[PortalPaneType-Active] portalPaneType on portalPaneType.TypeId = type.Id
        inner join #segmentsTable cte on cte.Id = pane.SegmentId
        left join [Content].[PagePane-Active] pagePane on pagePane.PaneId = pane.Id
        left join [Content].[BinderPane-Active] binderPane on binderPane.PaneId = pane.Id
        left join [Content].[CoursePane-Active] coursePane on coursePane.PaneId = pane.Id
    where portalPaneType.PortalId = @PortalId
        and (
            pane.PermissionId is null
            or exists (
                select 1
                from #sessionPermissions permission
                where permission.PermissionId = pane.PermissionId
            )
        )
    order by
        pane.SegmentId,
        pane.Ordinal,
        pane.Id;
end