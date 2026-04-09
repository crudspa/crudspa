create trigger [Content].[ForumBundleTrigger] on [Content].[ForumBundle]
    for update
as

insert [Content].[ForumBundle] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ForumId
    ,BundleId
    ,ThreadRule
    ,CommentRule
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ForumId
    ,deleted.BundleId
    ,deleted.ThreadRule
    ,deleted.CommentRule
from deleted