create trigger [Education].[RosterSchoolLinkTrigger] on [Education].[RosterSchoolLink]
    for update
as

insert [Education].[RosterSchoolLink] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,RosterSourceId
    ,ExternalId
    ,SchoolId
    ,Included
    ,SourceHash
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.RosterSourceId
    ,deleted.ExternalId
    ,deleted.SchoolId
    ,deleted.Included
    ,deleted.SourceHash
from deleted