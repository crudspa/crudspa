create trigger [Content].[PublicationTrigger] on [Content].[Publication]
    for update
as

insert [Content].[Publication] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,GroupId
    ,StatusId
    ,Title
    ,Citation
    ,Venue
    ,Sample
    ,Timeline
    ,Outcome
    ,Intervention
    ,ImageId
    ,ImageAlt
    ,Year
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PortalId
    ,deleted.GroupId
    ,deleted.StatusId
    ,deleted.Title
    ,deleted.Citation
    ,deleted.Venue
    ,deleted.Sample
    ,deleted.Timeline
    ,deleted.Outcome
    ,deleted.Intervention
    ,deleted.ImageId
    ,deleted.ImageAlt
    ,deleted.Year
    ,deleted.Ordinal
from deleted