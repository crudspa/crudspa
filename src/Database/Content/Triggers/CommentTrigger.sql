create trigger [Content].[CommentTrigger] on [Content].[Comment]
    for update
as

insert [Content].[Comment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ParentId
    ,PostId
    ,ThreadId
    ,ById
    ,ByOrganizationName
    ,Posted
    ,Edited
    ,Removed
    ,Body
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ParentId
    ,deleted.PostId
    ,deleted.ThreadId
    ,deleted.ById
    ,deleted.ByOrganizationName
    ,deleted.Posted
    ,deleted.Edited
    ,deleted.Removed
    ,deleted.Body
from deleted