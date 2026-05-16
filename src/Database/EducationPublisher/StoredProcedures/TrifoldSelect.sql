create proc [EducationPublisher].[TrifoldSelect] (
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
     trifold.Id
    ,trifold.BookId
    ,book.[Key] as BookKey
    ,trifold.Title
    ,trifold.StatusId
    ,status.Name as StatusName
    ,trifold.RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,trifold.GeneratesAchievementId
    ,generatesAchievement.Title as GeneratesAchievementTitle
    ,trifold.Ordinal
    ,binder.Id as BinderId
    ,binder.TypeId as BinderTypeId
    ,type.Name as BinderTypeName
    ,guideBinder.Id as GuideBinderId
    ,guideBinder.BinderId as GuideBinderBinderId
    ,guideBinder.GuideImageId as GuideBinderGuideImageId
    ,guideImage.Id as GuideImageId
    ,guideImage.BlobId as GuideImageBlobId
    ,guideImage.Name as GuideImageName
    ,guideImage.Format as GuideImageFormat
    ,guideImage.Width as GuideImageWidth
    ,guideImage.Height as GuideImageHeight
    ,guideImage.Caption as GuideImageCaption
from [Education].[Trifold-Active] trifold
    inner join [Content].[Binder-Active] binder on trifold.BinderId = binder.Id
    left join [Education].[GuideBinder-Active] guideBinder on binder.Id = guideBinder.BinderId
    left join [Framework].[ImageFile-Active] guideImage on guideBinder.GuideImageId = guideImage.Id
    inner join [Education].[Book-Active] book on trifold.BookId = book.Id
    left join [Education].[Achievement-Active] generatesAchievement on trifold.GeneratesAchievementId = generatesAchievement.Id
    inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    left join [Education].[Achievement-Active] requiresAchievement on trifold.RequiresAchievementId = requiresAchievement.Id
    inner join [Framework].[ContentStatus-Active] status on trifold.StatusId = status.Id
    inner join [Content].[BinderType-Active] type on binder.TypeId = type.Id
where trifold.Id = @Id
    and organization.Id = @organizationId