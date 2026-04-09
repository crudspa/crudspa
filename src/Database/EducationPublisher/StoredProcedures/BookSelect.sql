create proc [EducationPublisher].[BookSelect] (
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
     book.Id
    ,book.Title
    ,book.StatusId
    ,status.Name as StatusName
    ,book.[Key]
    ,book.Author
    ,book.Isbn
    ,book.Lexile
    ,book.SeasonId
    ,season.Name as SeasonName
    ,book.CategoryId
    ,category.Name as CategoryName
    ,book.RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,book.Summary
    ,coverImage.Id as CoverImageId
    ,coverImage.BlobId as CoverImageBlobId
    ,coverImage.Name as CoverImageName
    ,coverImage.Format as CoverImageFormat
    ,coverImage.Width as CoverImageWidth
    ,coverImage.Height as CoverImageHeight
    ,coverImage.Caption as CoverImageCaption
    ,guideImage.Id as GuideImageId
    ,guideImage.BlobId as GuideImageBlobId
    ,guideImage.Name as GuideImageName
    ,guideImage.Format as GuideImageFormat
    ,guideImage.Width as GuideImageWidth
    ,guideImage.Height as GuideImageHeight
    ,guideImage.Caption as GuideImageCaption
    ,(select count(1) from [Education].[Chapter-Active] where BookId = book.Id) as ChapterCount
    ,(select count(1) from [Education].[Game-Active] where BookId = book.Id) as GameCount
    ,(select count(1) from [Education].[Module-Active] where BookId = book.Id) as ModuleCount
    ,(select count(1) from [Education].[Trifold-Active] where BookId = book.Id) as TrifoldCount
    ,(select count(1) from [Content].[Page-Active] where BinderId = book.PrefaceBinderId) as PrefaceCount
from [Education].[Book-Active] book
    left join [Education].[BookCategory-Active] category on book.CategoryId = category.Id
    inner join [Framework].[ImageFile-Active] coverImage on book.CoverImageId = coverImage.Id
    left join [Framework].[ImageFile-Active] guideImage on book.GuideImageId = guideImage.Id
    inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    left join [Education].[Achievement-Active] requiresAchievement on book.RequiresAchievementId = requiresAchievement.Id
    inner join [Education].[BookSeason-Active] season on book.SeasonId = season.Id
    inner join [Framework].[ContentStatus-Active] status on book.StatusId = status.Id
where book.Id = @Id
    and organization.Id = @organizationId