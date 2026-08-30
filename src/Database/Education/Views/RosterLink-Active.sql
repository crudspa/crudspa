create view [Education].[RosterLink-Active] as

select rosterLink.Id
    ,rosterLink.RosterSourceId
    ,rosterLink.Kind
    ,rosterLink.ExternalId
    ,rosterLink.LocalId
    ,rosterLink.SourceHash
from [Education].[RosterLink] rosterLink
where rosterLink.IsDeleted = 0
    and rosterLink.VersionOf = rosterLink.Id