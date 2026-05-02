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
    ,SeoTitle
    ,SeoDescription
    ,SeoKeywords
    ,SeoImageId
    ,CanonicalBaseUrl
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
    ,deleted.SeoTitle
    ,deleted.SeoDescription
    ,deleted.SeoKeywords
    ,deleted.SeoImageId
    ,deleted.CanonicalBaseUrl
    ,deleted.FooterPageId
from deleted