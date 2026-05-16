create proc [EducationPublisher].[GuideBinderSelectForBinder] (
     @SessionId uniqueidentifier
    ,@BinderId uniqueidentifier
) as

set nocount on
select
     guideBinder.Id
    ,guideBinder.BinderId
    ,guideBinder.GuideImageId
    ,guideImage.Id as GuideImageId
    ,guideImage.BlobId as GuideImageBlobId
    ,guideImage.Name as GuideImageName
    ,guideImage.Format as GuideImageFormat
    ,guideImage.Width as GuideImageWidth
    ,guideImage.Height as GuideImageHeight
    ,guideImage.Caption as GuideImageCaption
from [Education].[GuideBinder-Active] guideBinder
    left join [Framework].[ImageFile-Active] guideImage on guideBinder.GuideImageId = guideImage.Id
where guideBinder.BinderId = @BinderId