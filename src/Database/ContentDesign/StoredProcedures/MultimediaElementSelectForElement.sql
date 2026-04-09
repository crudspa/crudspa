create proc [ContentDesign].[MultimediaElementSelectForElement] (
     @ElementId uniqueidentifier
) as

select
     multimediaElement.Id
    ,multimediaElement.ElementId
    ,container.Id as ContainerId
    ,container.DirectionId as ContainerDirectionId
    ,container.WrapId as ContainerWrapId
    ,container.JustifyContentId as ContainerJustifyContentId
    ,container.AlignItemsId as ContainerAlignItemsId
    ,container.AlignContentId as ContainerAlignContentId
    ,container.Gap as ContainerGap
from [Content].[MultimediaElement-Active] multimediaElement
    inner join [Content].[Container-Active] container on multimediaElement.ContainerId = container.Id
where multimediaElement.ElementId = @ElementId

select
     multimediaItem.Id
    ,multimediaItem.MultimediaElementId
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
    ,item.Id as ItemId
    ,item.BasisId as ItemBasisId
    ,item.BasisAmount as ItemBasisAmount
    ,item.Grow as ItemGrow
    ,item.Shrink as ItemShrink
    ,item.AlignSelfId as ItemAlignSelfId
    ,item.MaxWidth as ItemMaxWidth
    ,item.MinWidth as ItemMinWidth
    ,item.Width as ItemWidth
    ,multimediaItem.MediaTypeIndex
    ,audio.Id as AudioId
    ,audio.BlobId as AudioBlobId
    ,audio.Name as AudioName
    ,audio.Format as AudioFormat
    ,audio.OptimizedStatus as AudioOptimizedStatus
    ,audio.OptimizedBlobId as AudioOptimizedBlobId
    ,audio.OptimizedFormat as AudioOptimizedFormat
    ,button.Id as ButtonId
    ,button.Internal as ButtonInteral
    ,button.Path as ButtonPath
    ,button.Text as ButtonText
    ,button.ShapeIndex as ButtonShapeIndex
    ,button.GraphicIndex as ButtonGraphicIndex
    ,button.TextTypeIndex as ButtonTextTypeIndex
    ,button.OrientationIndex as ButtonOrientationIndex
    ,button.IconId as ButtonIconId
    ,buttonImage.Id as ButtonImageId
    ,buttonImage.BlobId as ButtonImageBlobId
    ,buttonImage.Name as ButtonImageName
    ,buttonImage.Format as ButtonImageFormat
    ,buttonImage.Width as ButtonImageWidth
    ,buttonImage.Height as ButtonImageHeight
    ,buttonImage.Caption as ButtonImageCaption
    ,buttonIcon.CssClass as ButtonIconCssClass
    ,buttonBox.Id as ButtonBoxId
    ,buttonBox.BackgroundColor as ButtonBoxBackgroundColor
    ,buttonBoxBackgroundImage.Id as ButtonBoxBackgroundImageId
    ,buttonBoxBackgroundImage.BlobId as ButtonBoxBackgroundImageBlobId
    ,buttonBoxBackgroundImage.Name as ButtonBoxBackgroundImageName
    ,buttonBoxBackgroundImage.Format as ButtonBoxBackgroundImageFormat
    ,buttonBoxBackgroundImage.Width as ButtonBoxBackgroundImageWidth
    ,buttonBoxBackgroundImage.Height as ButtonBoxBackgroundImageHeight
    ,buttonBoxBackgroundImage.Caption as ButtonBoxBackgroundImageCaption
    ,buttonBox.BorderColor as ButtonBoxBorderColor
    ,buttonBox.BorderRadius as ButtonBoxBorderRadius
    ,buttonBox.BorderThickness as ButtonBoxBorderThickness
    ,buttonBox.BorderThicknessTop as ButtonBoxBorderThicknessTop
    ,buttonBox.BorderThicknessLeft as ButtonBoxBorderThicknessLeft
    ,buttonBox.BorderThicknessRight as ButtonBoxBorderThicknessRight
    ,buttonBox.BorderThicknessBottom as ButtonBoxBorderThicknessBottom
    ,buttonBox.CustomFontIndex as ButtonBoxCustomFontIndex
    ,buttonBox.FontSize as ButtonBoxFontSize
    ,buttonBox.FontWeight as ButtonBoxFontWeight
    ,buttonBox.ForegroundColor as ButtonBoxForegroundColor
    ,buttonBox.MarginBottom as ButtonBoxMarginBottom
    ,buttonBox.MarginLeft as ButtonBoxMarginLeft
    ,buttonBox.MarginRight as ButtonBoxMarginRight
    ,buttonBox.MarginTop as ButtonBoxMarginTop
    ,buttonBox.PaddingBottom as ButtonBoxPaddingBottom
    ,buttonBox.PaddingLeft as ButtonBoxPaddingLeft
    ,buttonBox.PaddingRight as ButtonBoxPaddingRight
    ,buttonBox.PaddingTop as ButtonBoxPaddingTop
    ,buttonBox.ShadowBlurRadius as ButtonBoxShadowBlurRadius
    ,buttonBox.ShadowColor as ButtonBoxShadowColor
    ,buttonBox.ShadowOffsetX as ButtonBoxShadowOffsetX
    ,buttonBox.ShadowOffsetY as ButtonBoxShadowOffsetY
    ,buttonBox.ShadowSpreadRadius as ButtonBoxShadowSpreadRadius
    ,buttonBox.TextShadowBlurRadius as ButtonBoxTextShadowBlurRadius
    ,buttonBox.TextShadowColor as ButtonBoxTextShadowColor
    ,buttonBox.TextShadowOffsetX as ButtonBoxTextShadowOffsetX
    ,buttonBox.TextShadowOffsetY as ButtonBoxTextShadowOffsetY
    ,buttonBox.HeadingLineHeight as ButtonBoxHeadingLineHeight
    ,buttonBox.ParagraphLineHeight as ButtonBoxParagraphLineHeight
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,multimediaItem.Text
    ,video.Id as VideoId
    ,video.BlobId as VideoBlobId
    ,video.Name as VideoName
    ,video.Format as VideoFormat
    ,video.Width as VideoWidth
    ,video.Height as VideoHeight
    ,video.OptimizedStatus as VideoOptimizedStatus
    ,video.OptimizedBlobId as VideoOptimizedBlobId
    ,video.OptimizedFormat as VideoOptimizedFormat
    ,multimediaItem.Ordinal
from [Content].[MultimediaItem-Active] multimediaItem
    inner join [Content].[MultimediaElement-Active] multimediaElement on multimediaItem.MultimediaElementId = multimediaElement.Id
    left join [Content].[Box-Active] box on multimediaItem.BoxId = box.Id
    left join [Content].[Item-Active] item on multimediaItem.ItemId = item.Id
    left join [Framework].[AudioFile-Active] audio on multimediaItem.AudioId = audio.Id
    left join [Content].[Button-Active] button on multimediaItem.ButtonId = button.Id
    left join [Framework].[Icon-Active] buttonIcon on button.IconId = buttonIcon.Id
    left join [Framework].[ImageFile-Active] buttonImage on button.ImageId = buttonImage.Id
    left join [Framework].[ImageFile-Active] image on multimediaItem.ImageId = image.Id
    left join [Framework].[VideoFile-Active] video on multimediaItem.VideoId = video.Id
    left join [Framework].[ImageFile-Active] backgroundImage on box.BackgroundImageId = backgroundImage.Id
    left join [Content].[Box-Active] buttonBox on button.BoxId = buttonBox.Id
    left join [Framework].[ImageFile-Active] buttonBoxBackgroundImage on buttonBox.BackgroundImageId = buttonBoxBackgroundImage.Id
where multimediaElement.ElementId = @ElementId