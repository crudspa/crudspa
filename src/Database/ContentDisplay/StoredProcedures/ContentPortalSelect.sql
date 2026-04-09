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
    ,contentPortal.FooterPageId
from [Framework].[Portal-Active] portal
    inner join [Framework].[NavigationType-Active] navigationType on portal.NavigationTypeId = navigationType.Id
    inner join [Content].[ContentPortal-Active] contentPortal on portal.Id = contentPortal.Id
    inner join [Framework].[Organization-Active] owner on portal.OwnerId = owner.Id
    left join [Framework].[ImageFile-Active] brandingImage on contentPortal.BrandingImageId = brandingImage.Id
where portal.Id = @Id