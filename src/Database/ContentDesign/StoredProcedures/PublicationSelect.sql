create proc [ContentDesign].[PublicationSelect] (
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
     publication.Id
    ,publication.PortalId
    ,publication.Title
    ,publication.GroupId
    ,groupTable.Name as GroupName
    ,publication.StatusId
    ,status.Name as StatusName
    ,publication.Citation
    ,publication.Venue
    ,publication.Sample
    ,publication.Timeline
    ,publication.Outcome
    ,publication.Intervention
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,publication.ImageAlt
    ,publication.Year
    ,publication.Ordinal
from [Content].[Publication-Active] publication
    left join [Content].[PublicationGroup-Active] groupTable on publication.GroupId = groupTable.Id
    left join [Framework].[ImageFile-Active] image on publication.ImageId = image.Id
    inner join [Framework].[Portal-Active] portal on publication.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[ContentStatus-Active] status on publication.StatusId = status.Id
where publication.Id = @Id
    and organization.Id = @organizationId

select
     publicationResource.Id
    ,publicationResource.PublicationId
    ,publication.Title as PublicationTitle
    ,publicationResource.TypeId
    ,type.Name as TypeName
    ,publicationResource.Label
    ,publicationResource.Url
    ,pdf.Id as PdfId
    ,pdf.BlobId as PdfBlobId
    ,pdf.Name as PdfName
    ,pdf.Format as PdfFormat
    ,pdf.Description as PdfDescription
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,publicationResource.Ordinal
from [Content].[PublicationResource-Active] publicationResource
    left join [Framework].[ImageFile-Active] image on publicationResource.ImageId = image.Id
    left join [Framework].[PdfFile-Active] pdf on publicationResource.PdfId = pdf.Id
    inner join [Content].[Publication-Active] publication on publicationResource.PublicationId = publication.Id
    inner join [Content].[PublicationResourceType-Active] type on publicationResource.TypeId = type.Id
where publicationResource.PublicationId = @Id
    and exists (select 1 from Framework.[Portal-Active] resourcePortal where resourcePortal.Id = publication.PortalId and resourcePortal.OwnerId = @organizationId)
order by publicationResource.Ordinal, publicationResource.Id