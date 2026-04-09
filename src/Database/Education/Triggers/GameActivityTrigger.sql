create trigger [Education].[GameActivityTrigger] on [Education].[GameActivity]
    for update
as

insert [Education].[GameActivity] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,SectionId
    ,ActivityId
    ,ThemeWord
    ,Rigorous
    ,Multisyllabic
    ,GroupId
    ,TypeId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.SectionId
    ,deleted.ActivityId
    ,deleted.ThemeWord
    ,deleted.Rigorous
    ,deleted.Multisyllabic
    ,deleted.GroupId
    ,deleted.TypeId
    ,deleted.Ordinal
from deleted