create proc [ContentDisplay].[ForumRunUploadStageConsume] (
     @SessionId uniqueidentifier
    ,@ForumId uniqueidentifier
    ,@BlobId uniqueidentifier
    ,@Type int
    ,@LicenseIds [Framework].[IdList] readonly
) as
begin
    set nocount on;
    set xact_abort on;

    declare @completeStatusId uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c';
    declare @forumsPermissionId uniqueidentifier = '7e4b7332-0f6f-429a-a95e-d8a21a08e4b5';
    declare @sessionUserId uniqueidentifier;
    declare @sessionPortalId uniqueidentifier;
    declare @sessionOrganizationId uniqueidentifier;
    declare @now datetimeoffset(7) = sysdatetimeoffset();

    select
         @sessionUserId = session.UserId
        ,@sessionPortalId = session.PortalId
        ,@sessionOrganizationId = userTable.OrganizationId
    from [Framework].[Session-Active] session
    inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
    where session.Id = @SessionId and session.Ended is null;

    if @sessionUserId is null return;

    declare @hasForumsPermission bit = case when exists (
        select 1
        from [Framework].[UserRole-Active] userRole
        inner join [Framework].[RolePermission-Active] rolePermission on rolePermission.RoleId = userRole.RoleId
        inner join [Framework].[PortalPermission-Active] portalPermission
            on portalPermission.PermissionId = rolePermission.PermissionId and portalPermission.PortalId = @sessionPortalId
        where userRole.UserId = @sessionUserId and rolePermission.PermissionId = @forumsPermissionId
    ) then 1 else 0 end;

    update uploadStage with (rowlock)
    set Consumed = @now
    output inserted.BlobId, inserted.[Type], inserted.[Name], inserted.[Format], inserted.ContentType, inserted.Bytes
    from [Content].[ForumUploadStage] uploadStage
    where uploadStage.BlobId = @BlobId
        and uploadStage.SessionId = @SessionId
        and uploadStage.UserId = @sessionUserId
        and uploadStage.ForumId = @ForumId
        and uploadStage.[Type] = @Type
        and uploadStage.Consumed is null
        and uploadStage.Expires > @now
        and uploadStage.Bytes <= case uploadStage.[Type]
            when 0 then 52428800
            when 1 then 10485760
            when 2 then 26214400
            when 3 then 262144000
            else 0
        end
        and exists (
            select 1 from [Content].[Forum-Active] forum
            where forum.Id = @ForumId
                and (
                    (
                        (
                            forum.PortalId = @sessionPortalId
                            or (forum.AccessMode = 1 and exists (
                                select 1 from [Content].[ForumLicense-Active] crossPortalLicense
                                inner join @LicenseIds licenseId on licenseId.Id = crossPortalLicense.LicenseId
                                inner join [Framework].[License-Active] activeLicense on activeLicense.Id = licenseId.Id
                                where crossPortalLicense.ForumId = forum.Id
                            ))
                        )
                        and forum.StatusId = @completeStatusId
                        and (forum.PermissionId is null or exists (
                            select 1 from [Framework].[UserRole-Active] userRole
                            inner join [Framework].[RolePermission-Active] rolePermission on rolePermission.RoleId = userRole.RoleId
                            inner join [Framework].[PortalPermission-Active] portalPermission
                                on portalPermission.PermissionId = rolePermission.PermissionId and portalPermission.PortalId = @sessionPortalId
                            where userRole.UserId = @sessionUserId and rolePermission.PermissionId = forum.PermissionId
                        ))
                        and (forum.AccessMode = 0 or (forum.AccessMode = 1 and exists (
                            select 1 from [Content].[ForumLicense-Active] forumLicense
                            inner join @LicenseIds licenseId on licenseId.Id = forumLicense.LicenseId
                            inner join [Framework].[License-Active] activeLicense on activeLicense.Id = licenseId.Id
                            where forumLicense.ForumId = forum.Id
                        )))
                    )
                    or (
                        @hasForumsPermission = 1
                        and exists (
                            select 1
                            from [Framework].[Portal-Active] portal
                            where portal.Id = forum.PortalId
                                and portal.OwnerId = @sessionOrganizationId
                        )
                    )
                )
        );
end