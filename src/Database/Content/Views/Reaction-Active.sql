create view [Content].[Reaction-Active] as

select reaction.Id as Id
    ,reaction.ById as ById
    ,reaction.Reacted as Reacted
    ,reaction.Emoji as Emoji
from [Content].[Reaction] reaction
where 1=1
    and reaction.IsDeleted = 0
    and reaction.VersionOf = reaction.Id