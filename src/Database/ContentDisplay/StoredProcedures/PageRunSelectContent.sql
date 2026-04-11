create proc [ContentDisplay].[PageRunSelectContent] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
    ,@SectionId uniqueidentifier = null
) as

select
     page.Id as Id
    ,page.BinderId as BinderId
    ,page.Title as Title
    ,page.ShowNotebook as ShowNotebook
    ,page.ShowGuide as ShowGuide
    ,page.GuideText as GuideText
    ,guideAudio.Id as GuideAudioId
    ,guideAudio.Name as GuideAudioName
    ,guideAudio.Format as GuideAudioFormat
    ,guideAudio.OptimizedStatus as GuideAudioOptimizedStatus
    ,guideAudio.OptimizedBlobId as GuideAudioOptimizedBlobId
    ,guideAudio.OptimizedFormat as GuideAudioOptimizedFormat
    ,page.Ordinal as Ordinal
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
from [Content].[Page-Active] page
    left join [Content].[Box-Active] box on page.BoxId = box.Id
    left join [Framework].[AudioFile-Active] guideAudio on page.GuideAudioId = guideAudio.Id
    left join [Framework].[ImageFile-Active] backgroundImage on box.BackgroundImageId = backgroundImage.Id
where page.Id = @Id

select
     section.Id
    ,section.PageId
    ,container.Id
    ,container.DirectionId
    ,container.WrapId
    ,container.JustifyContentId
    ,container.AlignItemsId
    ,container.AlignContentId
    ,container.Gap
    ,section.Ordinal
    ,section.TypeId
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
from [Content].[Section-Active] section
    left join [Content].[Container-Active] container on section.ContainerId = container.Id
    left join [Content].[Box-Active] box on section.BoxId = box.Id
    left join [Framework].[ImageFile-Active] backgroundImage on box.BackgroundImageId = backgroundImage.Id
where section.PageId = @Id
    and (@SectionId is null or section.Id = @SectionId)
order by section.Ordinal

select
     element.Id
    ,element.SectionId
    ,element.TypeId
    ,item.Id as ItemId
    ,item.BasisId as ItemBasisId
    ,item.BasisAmount as ItemBasisAmount
    ,item.Grow as ItemGrow
    ,item.Shrink as ItemShrink
    ,item.AlignSelfId as ItemAlignSelfId
    ,item.MaxWidth as ItemMaxWidth
    ,item.MinWidth as ItemMinWidth
    ,item.Width as ItemWidth
    ,element.RequireInteraction
    ,element.Ordinal
    ,elementType.EditorView as ElementTypeEditorView
    ,elementType.DisplayView as ElementTypeDisplayView
    ,elementType.RepositoryClass as ElementTypeRepositoryClass
    ,elementType.OnlyChild as ElementTypeOnlyChild
    ,elementType.SupportsInteraction as ElementTypeSupportsInteraction
    ,elementType.IconId as ElementTypeIconId
    ,elementTypeIcon.CssClass as ElementTypeIconCssClass
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
from [Content].[Element-Active] element
    inner join [Content].[ElementType-Active] elementType on element.TypeId = elementType.Id
    left join [Framework].[Icon-Active] elementTypeIcon on elementType.IconId = elementTypeIcon.Id
    left join [Content].[Box-Active] box on element.BoxId = box.Id
    left join [Content].[Item-Active] item on element.ItemId = item.Id
    left join [Framework].[ImageFile-Active] backgroundImage on box.BackgroundImageId = backgroundImage.Id
    inner join [Content].[Section-Active] section on element.SectionId = section.Id
where section.PageId = @Id
    and (@SectionId is null or section.Id = @SectionId)

select
     audio.Id
    ,audio.ElementId
    ,fileTable.Id as FileId
    ,fileTable.BlobId as FileBlobId
    ,fileTable.Name as FileName
    ,fileTable.Format as FileFormat
    ,fileTable.OptimizedStatus as FileOptimizedStatus
    ,fileTable.OptimizedBlobId as FileOptimizedBlobId
    ,fileTable.OptimizedFormat as FileOptimizedFormat
from [Content].[AudioElement-Active] audio
    inner join [Framework].[AudioFile-Active] fileTable on audio.FileId = fileTable.Id
    inner join [Content].[Element-Active] element on audio.ElementId = element.Id
    inner join [Content].[Section-Active] section on element.SectionId = section.Id
where section.PageId = @Id
    and (@SectionId is null or section.Id = @SectionId)

select
     textElement.Id
    ,textElement.ElementId
    ,textElement.Text
from [Content].[TextElement-Active] textElement
    inner join [Content].[Element-Active] element on textElement.ElementId = element.Id
    inner join [Content].[Section-Active] section on element.SectionId = section.Id
where section.PageId = @Id
    and (@SectionId is null or section.Id = @SectionId)

select
     image.Id
    ,image.ElementId
    ,fileTable.Id as FileId
    ,fileTable.BlobId as FileBlobId
    ,fileTable.Name as FileName
    ,fileTable.Format as FileFormat
    ,fileTable.Width as FileWidth
    ,fileTable.Height as FileHeight
    ,fileTable.Caption as FileCaption
    ,image.HyperlinkUrl
from [Content].[ImageElement-Active] image
    inner join [Framework].[ImageFile-Active] fileTable on image.FileId = fileTable.Id
    inner join [Content].[Element-Active] element on image.ElementId = element.Id
    inner join [Content].[Section-Active] section on element.SectionId = section.Id
where section.PageId = @Id
    and (@SectionId is null or section.Id = @SectionId)

select
     buttonElement.Id
    ,buttonElement.ElementId
    ,button.Id as ButtonId
    ,button.Internal
    ,button.Path
    ,button.Text
    ,button.ShapeIndex
    ,button.GraphicIndex
    ,button.TextTypeIndex
    ,button.OrientationIndex
    ,button.IconId
    ,buttonImage.Id as ButtonImageId
    ,buttonImage.BlobId as ButtonImageBlobId
    ,buttonImage.Name as ButtonImageName
    ,buttonImage.Format as ButtonImageFormat
    ,buttonImage.Width as ButtonImageWidth
    ,buttonImage.Height as ButtonImageHeight
    ,buttonImage.Caption as ButtonImageCaption
    ,icon.CssClass as ButtonIconCssClass
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
from [Content].[ButtonElement-Active] buttonElement
    inner join [Content].[Button-Active] button on buttonElement.ButtonId = button.Id
    inner join [Content].[Element-Active] element on buttonElement.ElementId = element.Id
    inner join [Content].[Section-Active] section on element.SectionId = section.Id
    left join [Framework].[Icon-Active] icon on button.IconId = icon.Id
    left join [Framework].[ImageFile-Active] buttonImage on button.ImageId = buttonImage.Id
    left join [Content].[Box-Active] box on button.BoxId = box.Id
    left join [Framework].[ImageFile-Active] backgroundImage on box.BackgroundImageId = backgroundImage.Id
where section.PageId = @Id
    and (@SectionId is null or section.Id = @SectionId)

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
    inner join [Content].[Element-Active] element on multimediaElement.ElementId = element.Id
    inner join [Content].[Section-Active] section on element.SectionId = section.Id
    inner join [Content].[Container-Active] container on multimediaElement.ContainerId = container.Id
where section.PageId = @Id
    and (@SectionId is null or section.Id = @SectionId)

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
    inner join [Content].[Element-Active] element on multimediaElement.ElementId = element.Id
    inner join [Content].[Section-Active] section on element.SectionId = section.Id
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
where section.PageId = @Id
    and (@SectionId is null or section.Id = @SectionId)

select
     note.Id
    ,note.ElementId
    ,note.Instructions
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
    ,note.RequireText
    ,note.RequireImageSelection
from [Content].[NoteElement-Active] note
    left join [Framework].[ImageFile-Active] imageFile on note.ImageFileId = imageFile.Id
    inner join [Content].[Element-Active] element on note.ElementId = element.Id
    inner join [Content].[Section-Active] section on element.SectionId = section.Id
where section.PageId = @Id
    and (@SectionId is null or section.Id = @SectionId)

select
     noteImage.Id as Id
    ,noteImage.NoteId as NoteId
    ,noteImage.ImageFileId as ImageFileId
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
    ,noteImage.Ordinal
from [Content].[NoteImage-Active] noteImage
    left join [Framework].[ImageFile-Active] imageFile on noteImage.ImageFileId = imageFile.Id
    inner join [Content].[NoteElement-Active] note on noteImage.NoteId = note.Id
    inner join [Content].[Element-Active] element on note.ElementId = element.Id
    inner join [Content].[Section-Active] section on element.SectionId = section.Id
where section.PageId = @Id
    and (@SectionId is null or section.Id = @SectionId)

select
     pdf.Id
    ,pdf.ElementId
    ,fileTable.Id as FileId
    ,fileTable.BlobId as FileBlobId
    ,fileTable.Name as FileName
    ,fileTable.Format as FileFormat
    ,fileTable.Description as FileDescription
from [Content].[PdfElement-Active] pdf
    inner join [Framework].[PdfFile-Active] fileTable on pdf.FileId = fileTable.Id
    inner join [Content].[Element-Active] element on pdf.ElementId = element.Id
    inner join [Content].[Section-Active] section on element.SectionId = section.Id
where section.PageId = @Id
    and (@SectionId is null or section.Id = @SectionId)

select
     video.Id
    ,video.ElementId
    ,fileTable.Id as FileId
    ,fileTable.BlobId as FileBlobId
    ,fileTable.Name as FileName
    ,fileTable.Format as FileFormat
    ,fileTable.Width as FileWidth
    ,fileTable.Height as FileHeight
    ,fileTable.OptimizedStatus as FileOptimizedStatus
    ,fileTable.OptimizedBlobId as FileOptimizedBlobId
    ,fileTable.OptimizedFormat as FileOptimizedFormat
    ,video.AutoPlay
    ,poster.Id as PosterId
    ,poster.BlobId as PosterBlobId
    ,poster.Name as PosterName
    ,poster.Format as PosterFormat
    ,poster.Width as PosterWidth
    ,poster.Height as PosterHeight
    ,poster.Caption as PosterCaption
from [Content].[VideoElement-Active] video
    inner join [Framework].[VideoFile-Active] fileTable on video.FileId = fileTable.Id
    inner join [Content].[Element-Active] element on video.ElementId = element.Id
    inner join [Content].[Section-Active] section on element.SectionId = section.Id
    left join [Framework].[ImageFile-Active] poster on video.PosterId = poster.Id
where section.PageId = @Id
    and (@SectionId is null or section.Id = @SectionId)