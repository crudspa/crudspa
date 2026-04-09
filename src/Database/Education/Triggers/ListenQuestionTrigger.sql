create trigger [Education].[ListenQuestionTrigger] on [Education].[ListenQuestion]
    for update
as

insert [Education].[ListenQuestion] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ListenPartId
    ,Text
    ,AudioFileId
    ,IsPreview
    ,PageBreakBefore
    ,HasCorrectChoice
    ,RequireTextInput
    ,CategoryId
    ,ImageFileId
    ,TypeId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ListenPartId
    ,deleted.Text
    ,deleted.AudioFileId
    ,deleted.IsPreview
    ,deleted.PageBreakBefore
    ,deleted.HasCorrectChoice
    ,deleted.RequireTextInput
    ,deleted.CategoryId
    ,deleted.ImageFileId
    ,deleted.TypeId
    ,deleted.Ordinal
from deleted