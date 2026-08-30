create view [Education].[RosterPerson-Active] as

select rosterPerson.Id
    ,rosterPerson.RosterRunId
    ,rosterPerson.ExternalId
    ,rosterPerson.SisId
    ,rosterPerson.FirstName
    ,rosterPerson.LastName
    ,rosterPerson.Email
    ,rosterPerson.Status
    ,rosterPerson.AuthIssuer
    ,rosterPerson.AuthSubject
    ,rosterPerson.AssessmentLevel
    ,rosterPerson.SourceHash
from [Education].[RosterPerson] rosterPerson