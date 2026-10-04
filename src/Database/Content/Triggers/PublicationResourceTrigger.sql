create trigger [Content].[PublicationResourceTrigger] on [Content].[PublicationResource]
    for update
as

insert [Content].[PublicationResource] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PublicationId
    ,TypeId
    ,Label
    ,Url
    ,PdfId
    ,ImageId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PublicationId
    ,deleted.TypeId
    ,deleted.Label
    ,deleted.Url
    ,deleted.PdfId
    ,deleted.ImageId
    ,deleted.Ordinal
from deleted