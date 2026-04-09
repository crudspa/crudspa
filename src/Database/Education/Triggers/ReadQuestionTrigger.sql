create trigger [Education].[ReadQuestionTrigger] on [Education].[ReadQuestion]
    for update
as

insert [Education].[ReadQuestion] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ReadPartId
    ,Text
    ,AudioFileId
    ,IsPreview
    ,PageBreakBefore
    ,HasCorrectChoice
    ,RequireTextInput
    ,TypeId
    ,CategoryId
    ,ImageFileId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ReadPartId
    ,deleted.Text
    ,deleted.AudioFileId
    ,deleted.IsPreview
    ,deleted.PageBreakBefore
    ,deleted.HasCorrectChoice
    ,deleted.RequireTextInput
    ,deleted.TypeId
    ,deleted.CategoryId
    ,deleted.ImageFileId
    ,deleted.Ordinal
from deleted