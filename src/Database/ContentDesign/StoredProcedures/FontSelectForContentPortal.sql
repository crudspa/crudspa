create proc [ContentDesign].[FontSelectForContentPortal] (
     @SessionId uniqueidentifier
    ,@ContentPortalId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     font.Id
    ,font.ContentPortalId
    ,font.Name
    ,fileTable.Id as FileId
    ,fileTable.BlobId as FileBlobId
    ,fileTable.Name as FileName
    ,fileTable.Format as FileFormat
    ,fileTable.Description as FileDescription
from [Content].[Font-Active] font
    inner join [Framework].[FontFile-Active] fileTable on font.FileId = fileTable.Id
    inner join [Content].[ContentPortal-Active] contentPortal on font.ContentPortalId = contentPortal.Id
    inner join [Framework].[Portal-Active] portal on contentPortal.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where font.ContentPortalId = @ContentPortalId
    and organization.Id = @organizationId
order by font.Name