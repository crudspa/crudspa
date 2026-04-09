create view [Content].[ContentPortal-Active] as

select contentPortal.Id as Id
    ,contentPortal.PortalId as PortalId
    ,contentPortal.MaxWidth as MaxWidth
    ,contentPortal.StyleRevision as StyleRevision
    ,contentPortal.BrandingImageId as BrandingImageId
    ,contentPortal.FooterPageId as FooterPageId
from [Content].[ContentPortal] contentPortal
where 1=1
    and contentPortal.IsDeleted = 0
    and contentPortal.VersionOf = contentPortal.Id