create proc [EducationStudent].[SegmentSelectForSession] (
     @SessionId uniqueidentifier,
    @PortalId uniqueidentifier
) as
begin
    set nocount on;

    declare @contentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c';
    declare @sessionUserId uniqueidentifier = null;
    declare @sessionContactId uniqueidentifier = null;

    select
        @sessionUserId = session.UserId,
        @sessionContactId = userTable.ContactId
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

    create table #districtContactDistricts (
        Id uniqueidentifier not null primary key
    );

    insert into #districtContactDistricts (Id)
    select districtIds.DistrictId
    from (
        select districtContact.DistrictId
        from [Education].[DistrictContact-Active] districtContact
        where @sessionUserId is not null
            and districtContact.UserId = @sessionUserId

        union

        select districtContact.DistrictId
        from [Education].[DistrictContact-Active] districtContact
        where @sessionContactId is not null
            and districtContact.ContactId = @sessionContactId
    ) districtIds
    where districtIds.DistrictId is not null;

    create table #sessionSchools (
        Id uniqueidentifier not null primary key
    );

    insert into #sessionSchools (Id)
    select schoolIds.SchoolId
    from (
        select schoolContact.SchoolId
        from [Education].[SchoolContact-Active] schoolContact
        where @sessionUserId is not null
            and schoolContact.UserId = @sessionUserId

        union

        select schoolContact.SchoolId
        from [Education].[SchoolContact-Active] schoolContact
        where @sessionContactId is not null
            and schoolContact.ContactId = @sessionContactId

        union

        select family.SchoolId
        from [Education].[Student-Active] student
            inner join [Education].[Family-Active] family on family.Id = student.FamilyId
        where @sessionContactId is not null
            and student.ContactId = @sessionContactId
    ) schoolIds
    where schoolIds.SchoolId is not null;

    create table #sessionDistricts (
        Id uniqueidentifier not null primary key
    );

    insert into #sessionDistricts (Id)
    select districtIdSource.Id
    from (
        select districtContactDistrict.Id
        from #districtContactDistricts districtContactDistrict
        union
        select school.DistrictId
        from [Education].[School-Active] school
            inner join #sessionSchools sessionSchool on sessionSchool.Id = school.Id
    ) districtIdSource
    where districtIdSource.Id is not null;

    create table #sessionLicenses (
        LicenseId uniqueidentifier not null primary key
    );

    insert into #sessionLicenses (LicenseId)
    select distinct districtLicense.LicenseId
    from [Education].[DistrictLicense-Active] districtLicense
    where districtLicense.LicenseId is not null
        and exists (
            select 1
            from #sessionDistricts sessionDistrict
            where sessionDistrict.Id = districtLicense.DistrictId
        )
        and (
            districtLicense.AllSchools = 1
            or exists (
                select 1
                from #districtContactDistricts districtContactDistrict
                where districtContactDistrict.Id = districtLicense.DistrictId
            )
            or exists (
                select 1
                from [Education].[DistrictLicenseSchool-Active] districtLicenseSchool
                    inner join #sessionSchools sessionSchool on sessionSchool.Id = districtLicenseSchool.SchoolId
                where districtLicenseSchool.DistrictLicenseId = districtLicense.Id
            )
        );

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
        type.DisplayView as TypeDisplayView
    from [Framework].[Pane-Active] pane
        inner join [Framework].[PaneType-Active] type on pane.TypeId = type.Id
        inner join [Framework].[PortalPaneType-Active] portalPaneType on portalPaneType.TypeId = type.Id
        inner join #segmentsTable cte on cte.Id = pane.SegmentId
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