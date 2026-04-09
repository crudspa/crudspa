create trigger [Education].[PostTrigger] on [Education].[Post]
    for update
as

insert [Education].[Post] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ForumId
    ,ParentId
    ,Pinned
    ,Body
    ,AudioId
    ,ImageId
    ,PdfId
    ,VideoId
    ,ById
    ,ByOrganizationName
    ,Posted
    ,Edited
    ,Type
    ,GradeId
    ,SubjectId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ForumId
    ,deleted.ParentId
    ,deleted.Pinned
    ,deleted.Body
    ,deleted.AudioId
    ,deleted.ImageId
    ,deleted.PdfId
    ,deleted.VideoId
    ,deleted.ById
    ,deleted.ByOrganizationName
    ,deleted.Posted
    ,deleted.Edited
    ,deleted.Type
    ,deleted.GradeId
    ,deleted.SubjectId
from deleted