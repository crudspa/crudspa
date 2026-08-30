create view [Education].[RosterChange-Active] as

select rosterChange.Id
    ,rosterChange.RosterRunId
    ,rosterChange.Kind
    ,rosterChange.ExternalId
    ,rosterChange.LocalId
    ,rosterChange.Action
    ,rosterChange.Severity
    ,rosterChange.Code
from [Education].[RosterChange] rosterChange