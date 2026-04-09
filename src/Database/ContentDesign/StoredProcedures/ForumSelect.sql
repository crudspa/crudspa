create proc [ContentDesign].[ForumSelect] (
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
     forum.Id
    ,forum.PortalId
    ,portal.[Key] as PortalKey
    ,forum.Title
    ,forum.StatusId
    ,status.Name as StatusName
    ,forum.Description
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,forum.Ordinal
from [Content].[Forum-Active] forum
    left join [Framework].[ImageFile-Active] image on forum.ImageId = image.Id
    inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[ContentStatus-Active] status on forum.StatusId = status.Id
where forum.Id = @Id
    and organization.Id = @organizationId