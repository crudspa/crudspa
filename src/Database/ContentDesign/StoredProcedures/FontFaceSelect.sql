create proc [ContentDesign].[FontFaceSelect] (
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
     fontFace.Id
    ,fontFace.FontId
    ,fileTable.Id as FileId
    ,fileTable.BlobId as FileBlobId
    ,fileTable.Name as FileName
    ,fileTable.Format as FileFormat
    ,fileTable.Description as FileDescription
    ,fontFace.Style
    ,fontFace.WeightMin
    ,fontFace.WeightMax
from [Content].[FontFace-Active] fontFace
    inner join [Framework].[FontFile-Active] fileTable on fontFace.FileId = fileTable.Id
    inner join [Content].[Font-Active] font on fontFace.FontId = font.Id
    inner join [Content].[ContentPortal-Active] contentPortal on font.ContentPortalId = contentPortal.Id
    inner join [Framework].[Portal-Active] portal on contentPortal.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where fontFace.Id = @Id
    and organization.Id = @organizationId