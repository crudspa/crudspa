create proc [ContentDesign].[ContentPortalSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     contentPortal.Id
    ,contentPortal.MaxWidth
    ,contentPortal.StyleRevision
    ,brandingImage.Id as BrandingImageId
    ,brandingImage.BlobId as BrandingImageBlobId
    ,brandingImage.Name as BrandingImageName
    ,brandingImage.Format as BrandingImageFormat
    ,brandingImage.Width as BrandingImageWidth
    ,brandingImage.Height as BrandingImageHeight
    ,brandingImage.Caption as BrandingImageCaption
    ,portal.Id as PortalId
    ,portal.[Key] as PortalKey
    ,portal.Title as PortalTitle
    ,(select count(1) from [Framework].[Segment-Active] where PortalId = portal.Id and ParentId is null) as SegmentCount
    ,contentPortal.FooterPageId
    ,(select count(1) from [Content].[Achievement-Active] where PortalId = portal.Id) as AchievementCount
    ,(select count(1) from [Content].[Blog-Active] where PortalId = portal.Id) as BlogCount
    ,(select count(1) from [Content].[Forum-Active] where PortalId = portal.Id) as ForumCount
    ,(select count(1) from [Content].[Track-Active] where PortalId = portal.Id) as TrackCount
    ,(select count(1) from [Content].[Style-Active] where ContentPortalId = contentPortal.Id) as StyleCount
    ,(select count(1) from [Content].[Font-Active] where ContentPortalId = contentPortal.Id) as FontCount
from [Content].[ContentPortal-Active] contentPortal
    left join [Framework].[ImageFile-Active] brandingImage on contentPortal.BrandingImageId = brandingImage.Id
    inner join [Framework].[Portal-Active] portal on contentPortal.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where contentPortal.Id = @Id
    and portal.OwnerId = @organizationId

select
     portalFeature.Id
    ,portalFeature.PortalId
    ,portalFeature.[Key]
    ,portalFeature.Title
    ,portalFeature.IconId
    ,portalFeature.PermissionId
    ,icon.CssClass as IconCssClass
from [Framework].[PortalFeature-Active] portalFeature
    inner join [Framework].[Portal-Active] portal on portalFeature.PortalId = portal.Id
    inner join [Content].[ContentPortal-Active] contentPortal on contentPortal.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[Icon-Active] icon on portalFeature.IconId = icon.Id
where contentPortal.Id = @Id
    and portal.OwnerId = @organizationId