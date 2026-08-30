create view [Education].[RosterRole-Active] as

select rosterRole.Id
    ,rosterRole.RosterRunId
    ,rosterRole.ExternalId
    ,rosterRole.PersonExternalId
    ,rosterRole.SchoolExternalId
    ,rosterRole.Role
    ,rosterRole.Grade
    ,rosterRole.[Primary]
    ,rosterRole.SourceHash
from [Education].[RosterRole] rosterRole