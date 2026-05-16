create proc [ContentDesign].[CoursePaneUpdate] (
     @SessionId uniqueidentifier
    ,@PaneId uniqueidentifier
    ,@IdSource int
    ,@CourseId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on

if @IdSource is null and @CourseId is not null
    set @IdSource = 1

if @IdSource is not null and @IdSource not in (0, 1)
begin
    ;throw 50000, 'Pane IdSource is not supported.', 1
end

if @IdSource = 0
    set @CourseId = null

if not exists (
    select 1
    from [Framework].[Pane-Active] pane
        inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where pane.Id = @PaneId
        and organization.Id = @organizationId
)
begin
    ;throw 50000, 'Pane was not found.', 1
end

if @IdSource = 1 and @CourseId is null
begin
    ;throw 50000, 'Specific-course pane must include CourseId.', 1
end

if @CourseId is not null and not exists (select 1 from [Content].[Course-Active] where Id = @CourseId)
begin
    ;throw 50000, 'Pane CourseId does not reference an active Course.', 1
end

declare @coursePaneId uniqueidentifier = (
    select top 1 Id
    from [Content].[CoursePane-Active]
    where PaneId = @PaneId
    order by Id
)

if @IdSource is null
begin
    update [Content].[CoursePane]
    set  Updated = @now
        ,UpdatedBy = @SessionId
        ,IsDeleted = 1
    where PaneId = @PaneId
        and Id = VersionOf
        and IsDeleted = 0
end
else
begin
    if @coursePaneId is null
    begin
        set @coursePaneId = newid()

        insert [Content].[CoursePane] (Id, VersionOf, Updated, UpdatedBy, PaneId, IdSource, CourseId)
        values (@coursePaneId, @coursePaneId, @now, @SessionId, @PaneId, @IdSource, @CourseId)
    end
    else
    begin
        update [Content].[CoursePane]
        set  Updated = @now
            ,UpdatedBy = @SessionId
            ,IdSource = @IdSource
            ,CourseId = @CourseId
        where Id = @coursePaneId
            and (
                IdSource <> @IdSource
                or (CourseId <> @CourseId)
                or (CourseId is null and @CourseId is not null)
                or (CourseId is not null and @CourseId is null)
            )
    end

    update [Content].[CoursePane]
    set  Updated = @now
        ,UpdatedBy = @SessionId
        ,IsDeleted = 1
    where PaneId = @PaneId
        and Id = VersionOf
        and IsDeleted = 0
        and Id <> @coursePaneId
end