create view [Content].[CommentReaction-Active] as

select commentReaction.Id as Id
    ,commentReaction.CommentId as CommentId
    ,commentReaction.ReactionId as ReactionId
from [Content].[CommentReaction] commentReaction
where 1=1
    and commentReaction.IsDeleted = 0
    and commentReaction.VersionOf = commentReaction.Id