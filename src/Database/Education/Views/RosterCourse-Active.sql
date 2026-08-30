create view [Education].[RosterCourse-Active] as

select rosterCourse.Id
    ,rosterCourse.RosterRunId
    ,rosterCourse.ExternalId
    ,rosterCourse.SisId
    ,rosterCourse.Name
    ,rosterCourse.Number
    ,rosterCourse.Status
    ,rosterCourse.SourceHash
from [Education].[RosterCourse] rosterCourse