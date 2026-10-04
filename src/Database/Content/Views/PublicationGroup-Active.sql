create view [Content].[PublicationGroup-Active] as

select publicationGroup.Id as Id
    ,publicationGroup.PortalId as PortalId
    ,publicationGroup.Name as Name
    ,publicationGroup.Ordinal as Ordinal
from [Content].[PublicationGroup] publicationGroup
where 1=1
    and publicationGroup.IsDeleted = 0
    and publicationGroup.VersionOf = publicationGroup.Id