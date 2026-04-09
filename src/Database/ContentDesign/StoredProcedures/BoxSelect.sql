create proc [ContentDesign].[BoxSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on
select
     box.Id
    ,box.BackgroundColor
    ,backgroundImage.Id as BackgroundImageId
    ,backgroundImage.BlobId as BackgroundImageBlobId
    ,backgroundImage.Name as BackgroundImageName
    ,backgroundImage.Format as BackgroundImageFormat
    ,backgroundImage.Width as BackgroundImageWidth
    ,backgroundImage.Height as BackgroundImageHeight
    ,backgroundImage.Caption as BackgroundImageCaption
    ,box.BorderColor
    ,box.BorderRadius
    ,box.BorderThickness
    ,box.BorderThicknessTop
    ,box.BorderThicknessLeft
    ,box.BorderThicknessRight
    ,box.BorderThicknessBottom
    ,box.CustomFontIndex
    ,box.FontSize
    ,box.FontWeight
    ,box.ForegroundColor
    ,box.MarginBottom
    ,box.MarginLeft
    ,box.MarginRight
    ,box.MarginTop
    ,box.PaddingBottom
    ,box.PaddingLeft
    ,box.PaddingRight
    ,box.PaddingTop
    ,box.ShadowBlurRadius
    ,box.ShadowColor
    ,box.ShadowOffsetX
    ,box.ShadowOffsetY
    ,box.ShadowSpreadRadius
    ,box.TextShadowBlurRadius
    ,box.TextShadowColor
    ,box.TextShadowOffsetX
    ,box.TextShadowOffsetY
    ,box.HeadingLineHeight
    ,box.ParagraphLineHeight
from [Content].[Box-Active] box
    left join [Framework].[ImageFile-Active] backgroundImage on box.BackgroundImageId = backgroundImage.Id
where box.Id = @Id