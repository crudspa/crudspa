create trigger [Content].[PostReactionTrigger] on [Content].[PostReaction]
    for update
as

insert [Content].[PostReaction] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PostId
    ,ReactionId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PostId
    ,deleted.ReactionId
from deleted