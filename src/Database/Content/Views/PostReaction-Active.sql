create view [Content].[PostReaction-Active] as

select postReaction.Id as Id
    ,postReaction.PostId as PostId
    ,postReaction.ReactionId as ReactionId
from [Content].[PostReaction] postReaction
where 1=1
    and postReaction.IsDeleted = 0
    and postReaction.VersionOf = postReaction.Id