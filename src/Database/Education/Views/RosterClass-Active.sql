create view [Education].[RosterClass-Active] as

select rosterClass.Id
    ,rosterClass.RosterRunId
    ,rosterClass.ExternalId
    ,rosterClass.SisId
    ,rosterClass.SchoolExternalId
    ,rosterClass.CourseExternalId
    ,rosterClass.TermExternalId
    ,rosterClass.Name
    ,rosterClass.Grade
    ,rosterClass.Subject
    ,rosterClass.Status
    ,rosterClass.SmallClassroom
    ,rosterClass.SourceHash
from [Education].[RosterClass] rosterClass