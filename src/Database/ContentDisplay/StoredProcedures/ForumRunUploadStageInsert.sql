create proc [ContentDisplay].[ForumRunUploadStageInsert] (
     @SessionId uniqueidentifier
    ,@ForumId uniqueidentifier
    ,@BlobId uniqueidentifier
    ,@Type int
    ,@Name nvarchar(150)
    ,@Format nvarchar(10)
    ,@ContentType nvarchar(100)
    ,@Bytes bigint
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

    select
         @sessionUserId = session.UserId
        ,@sessionPortalId = session.PortalId
        ,@sessionOrganizationId = userTable.OrganizationId
    from [Framework].[Session-Active] session
    inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
    where session.Id = @SessionId and session.Ended is null;

    declare @hasForumsPermission bit = case when exists (
        select 1
        from [Framework].[UserRole-Active] userRole
        inner join [Framework].[RolePermission-Active] rolePermission on rolePermission.RoleId = userRole.RoleId
        inner join [Framework].[PortalPermission-Active] portalPermission
            on portalPermission.PermissionId = rolePermission.PermissionId and portalPermission.PortalId = @sessionPortalId
        where userRole.UserId = @sessionUserId and rolePermission.PermissionId = @forumsPermissionId
    ) then 1 else 0 end;

    if @sessionUserId is null or not exists (
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
    ) return;

    if @BlobId is null or @Name is null or len(@Name) = 0 or @Format is null or @ContentType is null
        return;

    if @Type not in (0, 1, 2, 3) or @Bytes <= 0 or @Bytes > 4294967296 return;

    if not (
        (@Type = 0 and lower(@Format) in (N'.mp3', N'.m4a', N'.aac', N'.wav', N'.flac', N'.ogg', N'.oga', N'.opus', N'.aif', N'.aiff', N'.wma')
            and lower(@ContentType) in (N'audio/mpeg', N'audio/mp4', N'audio/aac', N'audio/x-aac', N'audio/wav', N'audio/x-wav', N'audio/flac', N'audio/ogg', N'application/ogg', N'audio/opus', N'audio/x-aiff', N'audio/x-ms-wma'))
        or (@Type = 1 and lower(@Format) in (N'.jpg', N'.jpeg', N'.png', N'.gif', N'.bmp', N'.tif', N'.tiff', N'.webp')
            and lower(@ContentType) in (N'image/jpeg', N'image/png', N'image/gif', N'image/bmp', N'image/tiff', N'image/webp'))
        or (@Type = 2 and lower(@Format) = N'.pdf' and lower(@ContentType) = N'application/pdf')
        or (@Type = 3 and lower(@Format) in (N'.mp4', N'.mov', N'.m4v', N'.avi', N'.wmv', N'.flv', N'.webm', N'.mkv', N'.mpeg', N'.mpg', N'.ts', N'.m2ts')
            and lower(@ContentType) in (N'video/mp4', N'video/quicktime', N'video/x-m4v', N'video/x-msvideo', N'video/x-ms-wmv', N'video/x-flv', N'video/webm', N'video/x-matroska', N'video/mpeg', N'video/mp2t', N'application/octet-stream'))
    ) return;

    declare @now datetimeoffset(7) = sysdatetimeoffset();
    declare @dayAgo datetimeoffset(7) = dateadd(day, -1, @now);
    declare @startedTransaction bit = 0;
    if @@trancount = 0
    begin
        set @startedTransaction = 1;
        begin transaction;
    end

    declare @activeStageCount int;
    declare @activeStageBytes bigint;

    select
         @activeStageCount = count(1)
        ,@activeStageBytes = coalesce(sum(Bytes), 0)
    from [Content].[ForumUploadStage] with (updlock, serializable)
    where UserId = @sessionUserId
        and ForumId = @ForumId
        and Consumed is null
        and Expires > @now;

    if @activeStageCount >= 20 or @activeStageBytes + @Bytes > 85899345920
    begin
        if @startedTransaction = 1 rollback transaction;
        select 2 as Result;
        return;
    end

    declare @dailyStageCount int;
    declare @dailyStageBytes bigint;

    select
         @dailyStageCount = count(1)
        ,@dailyStageBytes = coalesce(sum(Bytes), 0)
    from [Content].[ForumUploadStage] with (updlock, serializable)
    where UserId = @sessionUserId
        and ForumId = @ForumId
        and Staged >= @dayAgo;

    if @dailyStageCount >= 40 or @dailyStageBytes + @Bytes > 171798691840
    begin
        if @startedTransaction = 1 rollback transaction;
        select 3 as Result;
        return;
    end

    insert [Content].[ForumUploadStage] (BlobId, SessionId, UserId, ForumId, [Type], [Name], [Format], ContentType, Bytes, Staged, Expires)
    values (@BlobId, @SessionId, @sessionUserId, @ForumId, @Type, @Name, lower(@Format), lower(@ContentType), @Bytes, @now, dateadd(minute, 30, @now));

    if @startedTransaction = 1 commit transaction;
    select 1 as Result;
end