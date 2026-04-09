create trigger [Content].[ContentPortalTrigger] on [Content].[ContentPortal]
    for update
as

insert [Content].[ContentPortal] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,MaxWidth
    ,StyleRevision
    ,BrandingImageId
    ,FooterPageId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PortalId
    ,deleted.MaxWidth
    ,deleted.StyleRevision
    ,deleted.BrandingImageId
    ,deleted.FooterPageId
from deleted