create trigger [Content].[ReactionTrigger] on [Content].[Reaction]
    for update
as

insert [Content].[Reaction] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ById
    ,Reacted
    ,Emoji
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ById
    ,deleted.Reacted
    ,deleted.Emoji
from deleted