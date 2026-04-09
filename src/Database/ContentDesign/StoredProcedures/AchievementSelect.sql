create proc [ContentDesign].[AchievementSelect] (
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
     achievement.Id
    ,achievement.PortalId
    ,achievement.Title
    ,achievement.Description
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
from [Content].[Achievement-Active] achievement
    inner join [Framework].[ImageFile-Active] image on achievement.ImageId = image.Id
    inner join [Framework].[Portal-Active] portal on achievement.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where achievement.Id = @Id
    and organization.Id = @organizationId