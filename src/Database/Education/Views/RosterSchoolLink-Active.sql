create view [Education].[RosterSchoolLink-Active] as

select rosterSchoolLink.Id
    ,rosterSchoolLink.RosterSourceId
    ,rosterSchoolLink.ExternalId
    ,rosterSchoolLink.SchoolId
    ,rosterSchoolLink.Included
    ,rosterSchoolLink.SourceHash
from [Education].[RosterSchoolLink] rosterSchoolLink
where rosterSchoolLink.IsDeleted = 0
    and rosterSchoolLink.VersionOf = rosterSchoolLink.Id