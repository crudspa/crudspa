create view [Content].[Box-Active] as

select box.Id as Id
    ,box.BackgroundColor as BackgroundColor
    ,box.BackgroundImageId as BackgroundImageId
    ,box.BorderColor as BorderColor
    ,box.BorderRadius as BorderRadius
    ,box.BorderThickness as BorderThickness
    ,box.BorderThicknessTop as BorderThicknessTop
    ,box.BorderThicknessLeft as BorderThicknessLeft
    ,box.BorderThicknessRight as BorderThicknessRight
    ,box.BorderThicknessBottom as BorderThicknessBottom
    ,box.CustomFontIndex as CustomFontIndex
    ,box.FontSize as FontSize
    ,box.FontWeight as FontWeight
    ,box.ForegroundColor as ForegroundColor
    ,box.MarginBottom as MarginBottom
    ,box.MarginLeft as MarginLeft
    ,box.MarginRight as MarginRight
    ,box.MarginTop as MarginTop
    ,box.PaddingBottom as PaddingBottom
    ,box.PaddingLeft as PaddingLeft
    ,box.PaddingRight as PaddingRight
    ,box.PaddingTop as PaddingTop
    ,box.ShadowBlurRadius as ShadowBlurRadius
    ,box.ShadowColor as ShadowColor
    ,box.ShadowOffsetX as ShadowOffsetX
    ,box.ShadowOffsetY as ShadowOffsetY
    ,box.ShadowSpreadRadius as ShadowSpreadRadius
    ,box.TextShadowBlurRadius as TextShadowBlurRadius
    ,box.TextShadowColor as TextShadowColor
    ,box.TextShadowOffsetX as TextShadowOffsetX
    ,box.TextShadowOffsetY as TextShadowOffsetY
    ,box.HeadingLineHeight as HeadingLineHeight
    ,box.ParagraphLineHeight as ParagraphLineHeight
from [Content].[Box] box
where 1=1
    and box.IsDeleted = 0
    and box.VersionOf = box.Id