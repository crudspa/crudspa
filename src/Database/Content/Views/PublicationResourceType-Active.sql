create view [Content].[PublicationResourceType-Active] as

select publicationResourceType.Id as Id
    ,publicationResourceType.Name as Name
    ,publicationResourceType.Ordinal as Ordinal
from [Content].[PublicationResourceType] publicationResourceType
where 1=1
    and publicationResourceType.IsDeleted = 0