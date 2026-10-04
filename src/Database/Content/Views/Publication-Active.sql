create view [Content].[Publication-Active] as

select publication.Id as Id
    ,publication.PortalId as PortalId
    ,publication.GroupId as GroupId
    ,publication.StatusId as StatusId
    ,publication.Title as Title
    ,publication.Citation as Citation
    ,publication.Venue as Venue
    ,publication.Sample as Sample
    ,publication.Timeline as Timeline
    ,publication.Outcome as Outcome
    ,publication.Intervention as Intervention
    ,publication.ImageId as ImageId
    ,publication.ImageAlt as ImageAlt
    ,publication.Year as Year
    ,publication.Ordinal as Ordinal
from [Content].[Publication] publication
where 1=1
    and publication.IsDeleted = 0
    and publication.VersionOf = publication.Id