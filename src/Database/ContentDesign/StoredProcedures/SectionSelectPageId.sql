create proc [ContentDesign].[SectionSelectPageId] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@PageId uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @PageId = (
    select top 1 section.PageId
    from [Content].[Section-Active] section
        inner join [Content].[Page-Active] page on section.PageId = page.Id
    where section.Id = @Id
        and (
            exists (
                select 1
                from [Content].[Course-Active] course
                    inner join [Content].[Track-Active] track on course.TrackId = track.Id
                    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
                where course.BinderId = page.BinderId
                    and portal.OwnerId = @organizationId
            )
            or exists (
                select 1
                from [Content].[ContentPortal-Active] contentPortal
                    inner join [Framework].[Portal-Active] portal on contentPortal.PortalId = portal.Id
                where contentPortal.FooterPageId = page.Id
                    and portal.OwnerId = @organizationId
            )
            or exists (
                select 1
                from [Framework].[Pane-Active] pane
                    inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
                    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
                where portal.OwnerId = @organizationId
                    and (
                        exists (
                            select 1
                            from [Content].[PagePane-Active] pagePane
                            where pagePane.PaneId = pane.Id
                                and pagePane.PageId = page.Id
                        )
                        or exists (
                            select 1
                            from [Content].[BinderPane-Active] binderPane
                            where binderPane.PaneId = pane.Id
                                and binderPane.BinderId = page.BinderId
                        )
                        or exists (
                            select 1
                            from [Content].[CoursePane-Active] coursePane
                                inner join [Content].[Course-Active] course on course.Id = coursePane.CourseId
                            where coursePane.PaneId = pane.Id
                                and coursePane.IdSource = 1
                                and course.BinderId = page.BinderId
                        )
                    )
            )
        )
)