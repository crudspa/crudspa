create view [Framework].[LinkFollowed-Active] as

select linkFollowed.Id as Id
    ,linkFollowed.Url as Url
    ,linkFollowed.Followed as Followed
    ,linkFollowed.FollowedBy as FollowedBy
from [Framework].[LinkFollowed] linkFollowed
where 1=1