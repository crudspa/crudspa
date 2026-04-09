create proc [EducationPublisher].[AchievementSelect] (
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
    ,achievement.Title
    ,achievement.RarityId
    ,rarity.Name as RarityName
    ,trophyImage.Id as TrophyImageId
    ,trophyImage.BlobId as TrophyImageBlobId
    ,trophyImage.Name as TrophyImageName
    ,trophyImage.Format as TrophyImageFormat
    ,trophyImage.Width as TrophyImageWidth
    ,trophyImage.Height as TrophyImageHeight
    ,trophyImage.Caption as TrophyImageCaption
    ,achievement.VisibleToStudents
from [Education].[Achievement-Active] achievement
    inner join [Framework].[Organization-Active] organization on achievement.OwnerId = organization.Id
    inner join [Education].[Rarity-Active] rarity on achievement.RarityId = rarity.Id
    left join [Framework].[ImageFile-Active] trophyImage on achievement.TrophyImageId = trophyImage.Id
where achievement.Id = @Id
    and organization.Id = @organizationId