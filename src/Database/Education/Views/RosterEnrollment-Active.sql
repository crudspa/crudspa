create view [Education].[RosterEnrollment-Active] as

select rosterEnrollment.Id
    ,rosterEnrollment.RosterRunId
    ,rosterEnrollment.ExternalId
    ,rosterEnrollment.PersonExternalId
    ,rosterEnrollment.ClassExternalId
    ,rosterEnrollment.SchoolExternalId
    ,rosterEnrollment.Role
    ,rosterEnrollment.[Primary]
    ,rosterEnrollment.Status
    ,rosterEnrollment.SourceHash
from [Education].[RosterEnrollment] rosterEnrollment