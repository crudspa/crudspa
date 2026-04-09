create view [Content].[ElementLinkFollowed-Active] as

select elementLinkFollowed.Id as Id
    ,elementLinkFollowed.ElementId as ElementId
    ,elementLinkFollowed.Url as Url
    ,elementLinkFollowed.Followed as Followed
    ,elementLinkFollowed.FollowedBy as FollowedBy
from [Content].[ElementLinkFollowed] elementLinkFollowed
where 1=1