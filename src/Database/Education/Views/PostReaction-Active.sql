create view [Education].[PostReaction-Active] as

select postReaction.Id as Id
    ,postReaction.PostId as PostId
    ,postReaction.ById as ById
    ,postReaction.Character as Character
    ,postReaction.Reacted as Reacted
from [Education].[PostReaction] postReaction
where 1=1
    and postReaction.IsDeleted = 0