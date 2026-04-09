create proc [ContentDesign].[PageSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on
select
     page.Id
    ,page.BinderId
    ,page.Title
    ,page.TypeId
    ,type.Name as TypeName
    ,page.StatusId
    ,status.Name as StatusName
    ,page.GuideText
    ,guideAudio.Id as GuideAudioId
    ,guideAudio.BlobId as GuideAudioBlobId
    ,guideAudio.Name as GuideAudioName
    ,guideAudio.Format as GuideAudioFormat
    ,guideAudio.OptimizedStatus as GuideAudioOptimizedStatus
    ,guideAudio.OptimizedBlobId as GuideAudioOptimizedBlobId
    ,guideAudio.OptimizedFormat as GuideAudioOptimizedFormat
    ,page.ShowNotebook
    ,page.ShowGuide
    ,page.Ordinal
    ,box.Id as BoxId
    ,box.BackgroundColor as BoxBackgroundColor
    ,backgroundImage.Id as BoxBackgroundImageId
    ,backgroundImage.BlobId as BoxBackgroundImageBlobId
    ,backgroundImage.Name as BoxBackgroundImageName
    ,backgroundImage.Format as BoxBackgroundImageFormat
    ,backgroundImage.Width as BoxBackgroundImageWidth
    ,backgroundImage.Height as BoxBackgroundImageHeight
    ,backgroundImage.Caption as BoxBackgroundImageCaption
    ,box.BorderColor as BoxBorderColor
    ,box.BorderRadius as BoxBorderRadius
    ,box.BorderThickness as BoxBorderThickness
    ,box.BorderThicknessTop as BoxBorderThicknessTop
    ,box.BorderThicknessLeft as BoxBorderThicknessLeft
    ,box.BorderThicknessRight as BoxBorderThicknessRight
    ,box.BorderThicknessBottom as BoxBorderThicknessBottom
    ,box.CustomFontIndex as BoxCustomFontIndex
    ,box.FontSize as BoxFontSize
    ,box.FontWeight as BoxFontWeight
    ,box.ForegroundColor as BoxForegroundColor
    ,box.MarginBottom as BoxMarginBottom
    ,box.MarginLeft as BoxMarginLeft
    ,box.MarginRight as BoxMarginRight
    ,box.MarginTop as BoxMarginTop
    ,box.PaddingBottom as BoxPaddingBottom
    ,box.PaddingLeft as BoxPaddingLeft
    ,box.PaddingRight as BoxPaddingRight
    ,box.PaddingTop as BoxPaddingTop
    ,box.ShadowBlurRadius as BoxShadowBlurRadius
    ,box.ShadowColor as BoxShadowColor
    ,box.ShadowOffsetX as BoxShadowOffsetX
    ,box.ShadowOffsetY as BoxShadowOffsetY
    ,box.ShadowSpreadRadius as BoxShadowSpreadRadius
    ,box.TextShadowBlurRadius as BoxTextShadowBlurRadius
    ,box.TextShadowColor as BoxTextShadowColor
    ,box.TextShadowOffsetX as BoxTextShadowOffsetX
    ,box.TextShadowOffsetY as BoxTextShadowOffsetY
    ,box.HeadingLineHeight as BoxHeadingLineHeight
    ,box.ParagraphLineHeight as BoxParagraphLineHeight
    ,(select count(1) from [Content].[Section-Active] where PageId = page.Id) as SectionCount
from [Content].[Page-Active] page
    left join [Content].[Box-Active] box on page.BoxId = box.Id
    left join [Framework].[AudioFile-Active] guideAudio on page.GuideAudioId = guideAudio.Id
    inner join [Framework].[ContentStatus-Active] status on page.StatusId = status.Id
    inner join [Content].[PageType-Active] type on page.TypeId = type.Id
    left join [Framework].[ImageFile-Active] backgroundImage on box.BackgroundImageId = backgroundImage.Id
where page.Id = @Id