create proc [EducationRostering].[RosterRunSelect] (
     @RosterRunId uniqueidentifier
) as

select
     rosterRun.Id
    ,rosterRun.RosterSourceId
    ,rosterRun.Kind
    ,rosterRun.Status
    ,rosterRun.Started
    ,rosterRun.Completed
    ,rosterRun.[Checkpoint]
    ,rosterRun.SchoolCount
    ,rosterRun.TermCount
    ,rosterRun.CourseCount
    ,rosterRun.UserCount
    ,rosterRun.RoleCount
    ,rosterRun.ClassCount
    ,rosterRun.EnrollmentCount
    ,rosterRun.AddCount
    ,rosterRun.UpdateCount
    ,rosterRun.RemoveCount
    ,rosterRun.IssueCount
from [Education].[RosterRun] rosterRun
where rosterRun.Id = @RosterRunId