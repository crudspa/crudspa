create proc [ContentDesign].[BoxUpsert] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@BackgroundColor nvarchar(10)
    ,@BackgroundImageId uniqueidentifier
    ,@BorderColor nvarchar(10)
    ,@BorderRadius nvarchar(10)
    ,@BorderThickness nvarchar(10)
    ,@BorderThicknessTop nvarchar(10)
    ,@BorderThicknessLeft nvarchar(10)
    ,@BorderThicknessRight nvarchar(10)
    ,@BorderThicknessBottom nvarchar(10)
    ,@CustomFontIndex int
    ,@FontSize nvarchar(10)
    ,@FontWeight nvarchar(10)
    ,@ForegroundColor nvarchar(10)
    ,@MarginBottom nvarchar(10)
    ,@MarginLeft nvarchar(10)
    ,@MarginRight nvarchar(10)
    ,@MarginTop nvarchar(10)
    ,@PaddingBottom nvarchar(10)
    ,@PaddingLeft nvarchar(10)
    ,@PaddingRight nvarchar(10)
    ,@PaddingTop nvarchar(10)
    ,@ShadowBlurRadius nvarchar(10)
    ,@ShadowColor nvarchar(10)
    ,@ShadowOffsetX nvarchar(10)
    ,@ShadowOffsetY nvarchar(10)
    ,@ShadowSpreadRadius nvarchar(10)
    ,@TextShadowBlurRadius nvarchar(10)
    ,@TextShadowColor nvarchar(10)
    ,@TextShadowOffsetX nvarchar(10)
    ,@TextShadowOffsetY nvarchar(10)
    ,@HeadingLineHeight nvarchar(10)
    ,@ParagraphLineHeight nvarchar(10)
) as

declare @now datetimeoffset = sysdatetimeoffset()

if not exists (select Id from [Content].[Box-Active] where Id = @Id)
begin
    insert [Content].[Box] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,BackgroundColor
        ,BackgroundImageId
        ,BorderColor
        ,BorderRadius
        ,BorderThickness
        ,BorderThicknessTop
        ,BorderThicknessLeft
        ,BorderThicknessRight
        ,BorderThicknessBottom
        ,CustomFontIndex
        ,FontSize
        ,FontWeight
        ,ForegroundColor
        ,MarginBottom
        ,MarginLeft
        ,MarginRight
        ,MarginTop
        ,PaddingBottom
        ,PaddingLeft
        ,PaddingRight
        ,PaddingTop
        ,ShadowBlurRadius
        ,ShadowColor
        ,ShadowOffsetX
        ,ShadowOffsetY
        ,ShadowSpreadRadius
        ,TextShadowBlurRadius
        ,TextShadowColor
        ,TextShadowOffsetX
        ,TextShadowOffsetY
        ,HeadingLineHeight
        ,ParagraphLineHeight
    )
    values (
        @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@BackgroundColor
        ,@BackgroundImageId
        ,@BorderColor
        ,@BorderRadius
        ,@BorderThickness
        ,@BorderThicknessTop
        ,@BorderThicknessLeft
        ,@BorderThicknessRight
        ,@BorderThicknessBottom
        ,@CustomFontIndex
        ,@FontSize
        ,@FontWeight
        ,@ForegroundColor
        ,@MarginBottom
        ,@MarginLeft
        ,@MarginRight
        ,@MarginTop
        ,@PaddingBottom
        ,@PaddingLeft
        ,@PaddingRight
        ,@PaddingTop
        ,@ShadowBlurRadius
        ,@ShadowColor
        ,@ShadowOffsetX
        ,@ShadowOffsetY
        ,@ShadowSpreadRadius
        ,@TextShadowBlurRadius
        ,@TextShadowColor
        ,@TextShadowOffsetX
        ,@TextShadowOffsetY
        ,@HeadingLineHeight
        ,@ParagraphLineHeight
    )
end
else
begin
    update [Content].[Box]
    set
        Id = @Id
        ,Updated = @now
        ,UpdatedBy = @SessionId
        ,BackgroundColor = @BackgroundColor
        ,BackgroundImageId = @BackgroundImageId
        ,BorderColor = @BorderColor
        ,BorderRadius = @BorderRadius
        ,BorderThickness = @BorderThickness
        ,BorderThicknessTop = @BorderThicknessTop
        ,BorderThicknessLeft = @BorderThicknessLeft
        ,BorderThicknessRight = @BorderThicknessRight
        ,BorderThicknessBottom = @BorderThicknessBottom
        ,CustomFontIndex = @CustomFontIndex
        ,FontSize = @FontSize
        ,FontWeight = @FontWeight
        ,ForegroundColor = @ForegroundColor
        ,MarginBottom = @MarginBottom
        ,MarginLeft = @MarginLeft
        ,MarginRight = @MarginRight
        ,MarginTop = @MarginTop
        ,PaddingBottom = @PaddingBottom
        ,PaddingLeft = @PaddingLeft
        ,PaddingRight = @PaddingRight
        ,PaddingTop = @PaddingTop
        ,ShadowBlurRadius = @ShadowBlurRadius
        ,ShadowColor = @ShadowColor
        ,ShadowOffsetX = @ShadowOffsetX
        ,ShadowOffsetY = @ShadowOffsetY
        ,ShadowSpreadRadius = @ShadowSpreadRadius
        ,TextShadowBlurRadius = @TextShadowBlurRadius
        ,TextShadowColor = @TextShadowColor
        ,TextShadowOffsetX = @TextShadowOffsetX
        ,TextShadowOffsetY = @TextShadowOffsetY
        ,HeadingLineHeight = @HeadingLineHeight
        ,ParagraphLineHeight = @ParagraphLineHeight
    where Id = @Id
end