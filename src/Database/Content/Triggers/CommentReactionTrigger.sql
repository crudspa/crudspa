create trigger [Content].[CommentReactionTrigger] on [Content].[CommentReaction]
    for update
as

insert [Content].[CommentReaction] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,CommentId
    ,ReactionId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.CommentId
    ,deleted.ReactionId
from deleted