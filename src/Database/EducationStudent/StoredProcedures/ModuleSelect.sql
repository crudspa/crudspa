create proc [EducationStudent].[ModuleSelect] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

insert [Education].[ModuleViewed] (
     Id
    ,Updated
    ,UpdatedBy
    ,ModuleId
)
values (
     newid()
    ,@now
    ,@SessionId
    ,@Id
)

select
     module.Id
    ,module.Title
    ,icon.CssClass as IconName
    ,module.BookId
    ,module.StatusId
    ,module.BinderId
    ,module.Ordinal
    ,book.Title as BookTitle
    ,bookGuideImage.Id as BookGuideImageId
    ,bookGuideImage.BlobId as BookGuideImageBlobId
    ,bookGuideImage.Name as BookGuideImageName
    ,bookGuideImage.Format as BookGuideImageFormat
    ,bookGuideImage.Width as BookGuideImageWidth
    ,bookGuideImage.Height as BookGuideImageHeight
    ,bookGuideImage.Caption as BookGuideImageCaption
    ,status.Name as StatusName
    ,binderType.DisplayView as BinderDisplayView
from [Education].[Module-Active] module
    inner join [Education].[Book-Active] book on module.BookId = book.Id
    left join [Framework].[ImageFile-Active] bookGuideImage on book.GuideImageId = bookGuideImage.Id
    inner join [Framework].[ContentStatus-Active] status on module.StatusId = status.Id
    left join [Framework].[Icon-Active] icon on module.IconId = icon.Id
    inner join [Content].[Binder] binder on module.BinderId = binder.Id
    inner join [Content].[BinderType-Active] binderType on binder.TypeId = binderType.Id
where module.Id = @Id
    and module.StatusId = @ContentStatusComplete