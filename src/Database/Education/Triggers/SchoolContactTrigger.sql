create trigger [Education].[SchoolContactTrigger] on [Education].[SchoolContact]
    for update
as

insert [Education].[SchoolContact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,SchoolId
    ,ContactId
    ,UserId
    ,TitleId
    ,TestAccount
    ,Treatment
    ,IdNumber
    ,ResearchGroupId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.SchoolId
    ,deleted.ContactId
    ,deleted.UserId
    ,deleted.TitleId
    ,deleted.TestAccount
    ,deleted.Treatment
    ,deleted.IdNumber
    ,deleted.ResearchGroupId
from deleted