create proc [ContentDisplay].[ContentPortalSelect] (
     @Id uniqueidentifier
) as

set nocount on

select
     contentPortal.Id
    ,portal.Id
    ,portal.[Key]
    ,portal.Title
    ,portal.SessionsPersist
    ,portal.AllowSignIn
    ,portal.RequireSignIn
    ,navigationType.DisplayView as NavigationTypeDisplayView
    ,contentPortal.MaxWidth
    ,brandingImage.Id as BrandingImageId
    ,brandingImage.BlobId as BrandingImageBlobId
    ,brandingImage.Name as BrandingImageName
    ,brandingImage.Format as BrandingImageFormat
    ,brandingImage.Width as BrandingImageWidth
    ,brandingImage.Height as BrandingImageHeight
    ,brandingImage.Caption as BrandingImageCaption
    ,contentPortal.SeoTitle
    ,contentPortal.SeoDescription
    ,contentPortal.SeoKeywords
    ,seoImage.Id as SeoImageId
    ,seoImage.BlobId as SeoImageBlobId
    ,seoImage.Name as SeoImageName
    ,seoImage.Format as SeoImageFormat
    ,seoImage.Width as SeoImageWidth
    ,seoImage.Height as SeoImageHeight
    ,seoImage.Caption as SeoImageCaption
    ,contentPortal.CanonicalBaseUrl
    ,contentPortal.FooterPageId
from [Framework].[Portal-Active] portal
    inner join [Framework].[NavigationType-Active] navigationType on portal.NavigationTypeId = navigationType.Id
    inner join [Content].[ContentPortal-Active] contentPortal on portal.Id = contentPortal.Id
    inner join [Framework].[Organization-Active] owner on portal.OwnerId = owner.Id
    left join [Framework].[ImageFile-Active] brandingImage on contentPortal.BrandingImageId = brandingImage.Id
    left join [Framework].[ImageFile-Active] seoImage on contentPortal.SeoImageId = seoImage.Id
where portal.Id = @Id