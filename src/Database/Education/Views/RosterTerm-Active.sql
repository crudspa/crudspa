create view [Education].[RosterTerm-Active] as

select rosterTerm.Id
    ,rosterTerm.RosterRunId
    ,rosterTerm.ExternalId
    ,rosterTerm.SisId
    ,rosterTerm.Name
    ,rosterTerm.Kind
    ,rosterTerm.Starts
    ,rosterTerm.Ends
    ,rosterTerm.Status
    ,rosterTerm.SourceHash
from [Education].[RosterTerm] rosterTerm