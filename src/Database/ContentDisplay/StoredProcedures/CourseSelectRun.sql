create proc [ContentDisplay].[CourseSelectRun] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @portalId uniqueidentifier = (select top 1 PortalId from [Framework].[Session-Active] where Id = @SessionId)

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

insert [Content].[CourseViewed] (
     Id
    ,Updated
    ,UpdatedBy
    ,CourseId
)
values (
     newid()
    ,@now
    ,@SessionId
    ,@Id
)

select
     course.Id
    ,course.Title
    ,course.Description
    ,course.StatusId
    ,course.TrackId
    ,course.Ordinal
    ,track.Title as TrackTitle
    ,track.Description as TrackDescription
    ,binder.Id as BinderId
    ,binderType.DisplayView as BinderDisplayView
from [Content].[Course-Active] course
    inner join [Content].[Track-Active] track on course.TrackId = track.Id
    inner join [Content].[Binder] binder on course.BinderId = binder.Id
    inner join [Content].[BinderType-Active] binderType on binder.TypeId = binderType.Id
    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
where course.Id = @Id
    and course.StatusId = @ContentStatusComplete
    and portal.Id = @portalId