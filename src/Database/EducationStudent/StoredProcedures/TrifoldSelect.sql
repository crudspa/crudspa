create proc [EducationStudent].[TrifoldSelect] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

insert [Education].[TrifoldViewed] (
     Id
    ,Updated
    ,UpdatedBy
    ,TrifoldId
)
values (
     newid()
    ,@now
    ,@SessionId
    ,@Id
)

select
     trifold.Id
    ,trifold.Title
    ,trifold.BookId
    ,trifold.BinderId
    ,trifold.Ordinal
    ,guideImage.Id as GuideImageId
    ,guideImage.BlobId as GuideImageBlobId
    ,guideImage.Name as GuideImageName
    ,guideImage.Format as GuideImageFormat
    ,guideImage.Width as GuideImageWidth
    ,guideImage.Height as GuideImageHeight
    ,guideImage.Caption as GuideImageCaption
    ,unitbook.UnitId as UnitId
    ,binderType.DisplayView as BinderDisplayView
from [Education].[Trifold-Active] trifold
    left join [Education].[Book-Active] book on trifold.BookId = book.Id
    left join [Education].[UnitBook-Active] unitbook on book.Id = unitbook.BookId
    left join [Framework].[ImageFile-Active] guideImage on book.GuideImageId = guideImage.Id
    inner join [Content].[Binder] binder on trifold.BinderId = binder.Id
    inner join [Content].[BinderType-Active] binderType on binder.TypeId = binderType.Id
where trifold.Id = @Id
    and trifold.StatusId = @ContentStatusComplete