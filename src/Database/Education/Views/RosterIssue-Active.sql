create view [Education].[RosterIssue-Active] as

select rosterIssue.Id
    ,rosterIssue.RosterRunId
    ,rosterIssue.Kind
    ,rosterIssue.ExternalId
    ,rosterIssue.Severity
    ,rosterIssue.Code
    ,rosterIssue.Detail
from [Education].[RosterIssue] rosterIssue