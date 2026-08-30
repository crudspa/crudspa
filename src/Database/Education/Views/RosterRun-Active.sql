create view [Education].[RosterRun-Active] as

select rosterRun.Id as Id
    ,rosterRun.RosterSourceId as RosterSourceId
    ,rosterRun.Kind as Kind
    ,rosterRun.Status as Status
    ,rosterRun.Started as Started
    ,rosterRun.Completed as Completed
    ,rosterRun.[Checkpoint] as [Checkpoint]
    ,rosterRun.SchoolCount as SchoolCount
    ,rosterRun.UserCount as UserCount
    ,rosterRun.ClassCount as ClassCount
    ,rosterRun.EnrollmentCount as EnrollmentCount
    ,rosterRun.AddCount as AddCount
    ,rosterRun.UpdateCount as UpdateCount
    ,rosterRun.RemoveCount as RemoveCount
    ,rosterRun.IssueCount as IssueCount
    ,rosterRun.TermCount as TermCount
    ,rosterRun.CourseCount as CourseCount
    ,rosterRun.RoleCount as RoleCount
from [Education].[RosterRun] rosterRun
where 1=1