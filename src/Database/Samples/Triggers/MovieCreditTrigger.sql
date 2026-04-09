create trigger [Samples].[MovieCreditTrigger] on [Samples].[MovieCredit]
    for update
as

insert [Samples].[MovieCredit] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,MovieId
    ,Name
    ,Part
    ,Billing
    ,Headliner
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.MovieId
    ,deleted.Name
    ,deleted.Part
    ,deleted.Billing
    ,deleted.Headliner
    ,deleted.Ordinal
from deleted