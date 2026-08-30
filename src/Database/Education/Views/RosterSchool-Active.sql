create view [Education].[RosterSchool-Active] as

select rosterSchool.Id
    ,rosterSchool.RosterRunId
    ,rosterSchool.ExternalId
    ,rosterSchool.SisId
    ,rosterSchool.Name
    ,rosterSchool.Kind
    ,rosterSchool.Status
    ,rosterSchool.SourceHash
from [Education].[RosterSchool] rosterSchool