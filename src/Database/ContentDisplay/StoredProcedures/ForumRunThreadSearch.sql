create proc [ContentDisplay].[ForumRunThreadSearch] (
     @SessionId uniqueidentifier
    ,@ForumId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@PostedStart datetimeoffset(7)
    ,@PostedEnd datetimeoffset(7)
    ,@LicenseIds [Framework].[IdList] readonly
) as
begin
    set nocount on;

    set @PageNumber = case when @PageNumber < 1 then 1 else @PageNumber end;
    set @PageSize = case when @PageSize < 1 then 25 when @PageSize > 50 then 50 else @PageSize end;
    set @SortField = case when @SortField in ('Title', 'Posted', 'Activity') then @SortField else 'Activity' end;
    set @SortAscending = coalesce(@SortAscending, 0);

    declare @completeStatusId uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c';
    declare @forumsPermissionId uniqueidentifier = '7e4b7332-0f6f-429a-a95e-d8a21a08e4b5';
    declare @sessionUserId uniqueidentifier;
    declare @sessionContactId uniqueidentifier;
    declare @sessionPortalId uniqueidentifier;

    select @sessionUserId = session.UserId, @sessionPortalId = session.PortalId, @sessionContactId = userTable.ContactId
    from [Framework].[Session-Active] session
    left join [Framework].[User-Active] userTable on userTable.Id = session.UserId
    where session.Id = @SessionId and session.Ended is null;

    -- Runtime participants can modify only their own content; Publisher moderation uses ContentDesign.
    declare @canModerate bit = 0;

    if not exists (
        select 1 from [Content].[Forum-Active] forum
        where forum.Id = @ForumId
            and (
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
    ) return;

    declare @firstRecord bigint = (convert(bigint, @PageSize) * (@PageNumber - 1)) + 1;
    declare @lastRecord bigint = @firstRecord + @PageSize - 1;

    declare @threadRows table (
         RowNumber bigint not null
        ,TotalCount int not null
        ,Id uniqueidentifier not null primary key
    );

    ;with ThreadCte as (
        select
             row_number() over (order by
                thread.Pinned desc,
                case when @SortField = 'Title' and @SortAscending = 1 then thread.Title end asc,
                case when @SortField = 'Title' and @SortAscending = 0 then thread.Title end desc,
                case when @SortField = 'Posted' and @SortAscending = 1 then comment.Posted end asc,
                case when @SortField = 'Posted' and @SortAscending = 0 then comment.Posted end desc,
                case when (@SortField = 'Activity' or @SortField is null) and @SortAscending = 1 then activity.LastActivity end asc,
                case when (@SortField = 'Activity' or @SortField is null) and @SortAscending = 0 then activity.LastActivity end desc,
                thread.Id
             ) as RowNumber
            ,count(*) over () as TotalCount
            ,thread.Id
        from [Content].[Thread-Active] thread
        inner join [Content].[Comment-Active] comment on comment.Id = thread.CommentId
        outer apply (
            select max(source.Activity) as LastActivity
            from (
                select coalesce(comment.Edited, comment.Posted) as Activity
                union all
                select coalesce(reply.Edited, reply.Posted)
                from [Content].[Comment-Active] reply where reply.ThreadId = thread.Id
            ) source
        ) activity
        where thread.ForumId = @ForumId
            and (@SearchText is null or thread.Title like '%' + @SearchText + '%' or comment.Body like '%' + @SearchText + '%')
            and (@PostedStart is null or comment.Posted >= @PostedStart)
            and (@PostedEnd is null or comment.Posted < @PostedEnd)
    )
    insert @threadRows (RowNumber, TotalCount, Id)
    select RowNumber, TotalCount, Id
    from ThreadCte;

    select
         threadRow.TotalCount
        ,thread.Id
        ,thread.ForumId
        ,forum.Title
        ,thread.Title
        ,thread.Pinned
        ,comment.Id
        ,comment.Body
        ,comment.ById
        ,concat(byTable.FirstName, case when byTable.LastName is null then '' else ' ' + byTable.LastName end)
        ,comment.ByOrganizationName
        ,comment.Posted
        ,comment.Edited
        ,comment.Removed
        ,(select count(1) from [Content].[Comment-Active] reply where reply.ThreadId = thread.Id)
        ,activity.LastActivity
        ,convert(bit, case when @canModerate = 1 or comment.ById = @sessionContactId then 1 else 0 end)
        ,convert(bit, case when @canModerate = 1
            or (comment.ById = @sessionContactId and not exists (
                select 1 from [Content].[Comment-Active] reply where reply.ThreadId = thread.Id
            )) then 1 else 0 end)
        ,@canModerate
    from @threadRows threadRow
    inner join [Content].[Thread-Active] thread on thread.Id = threadRow.Id
    inner join [Content].[Forum-Active] forum on forum.Id = thread.ForumId
    inner join [Content].[Comment-Active] comment on comment.Id = thread.CommentId
    inner join [Framework].[Contact-Active] byTable on byTable.Id = comment.ById
    outer apply (
        select max(source.Activity) as LastActivity
        from (
            select coalesce(comment.Edited, comment.Posted) as Activity
            union all
            select coalesce(reply.Edited, reply.Posted)
            from [Content].[Comment-Active] reply where reply.ThreadId = thread.Id
        ) source
    ) activity
    where threadRow.RowNumber between @firstRecord and @lastRecord
    order by threadRow.RowNumber;

    select distinct
         threadTag.ThreadId
        ,tag.Id
        ,tag.Title
    from @threadRows threadRow
    inner join [Content].[ThreadTag-Active] threadTag on threadTag.ThreadId = threadRow.Id
    inner join [Content].[Tag-Active] tag on tag.Id = threadTag.TagId
    where threadRow.RowNumber between @firstRecord and @lastRecord
        and exists (
            select 1
            from [Content].[Thread-Active] thread
            inner join [Content].[ForumBundle-Active] forumBundle
                on forumBundle.ForumId = thread.ForumId and forumBundle.ThreadRule in (1, 2)
            inner join [Content].[TagBundle-Active] tagBundle
                on tagBundle.BundleId = forumBundle.BundleId and tagBundle.TagId = tag.Id
            where thread.Id = threadRow.Id
        )
    order by tag.Title;
end