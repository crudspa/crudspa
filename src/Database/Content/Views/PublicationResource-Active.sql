create view [Content].[PublicationResource-Active] as

select publicationResource.Id as Id
    ,publicationResource.PublicationId as PublicationId
    ,publicationResource.TypeId as TypeId
    ,publicationResource.Label as Label
    ,publicationResource.Url as Url
    ,publicationResource.PdfId as PdfId
    ,publicationResource.ImageId as ImageId
    ,publicationResource.Ordinal as Ordinal
from [Content].[PublicationResource] publicationResource
where 1=1
    and publicationResource.IsDeleted = 0
    and publicationResource.VersionOf = publicationResource.Id