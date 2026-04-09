create proc [ContentDesign].[ButtonUpsert] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Internal bit
    ,@Path nvarchar(250)
    ,@Text nvarchar(250)
    ,@ShapeIndex int
    ,@GraphicIndex int
    ,@TextTypeIndex int
    ,@OrientationIndex int
    ,@IconId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@BoxId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @effectiveText nvarchar(250) = case when @TextTypeIndex = 1 then @Text else null end
declare @effectiveIconId uniqueidentifier = case when @GraphicIndex = 1 then @IconId else null end
declare @effectiveImageId uniqueidentifier = case when @GraphicIndex = 2 then @ImageId else null end
declare @legacyLeftIconId uniqueidentifier = case when @GraphicIndex = 1 and @OrientationIndex = 0 then @IconId else null end
declare @legacyRightIconId uniqueidentifier = case when @GraphicIndex = 1 and @OrientationIndex = 1 then @IconId else null end

if not exists (select Id from [Content].[Button-Active] where Id = @Id)
begin
    insert [Content].[Button] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,Internal
        ,Path
        ,Text
        ,TextAlignIndex
        ,LeftIconId
        ,RightIconId
        ,BoxId
        ,ShapeIndex
        ,GraphicIndex
        ,TextTypeIndex
        ,OrientationIndex
        ,IconId
        ,ImageId
    )
    values (
        @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@Internal
        ,@Path
        ,@effectiveText
        ,null
        ,@legacyLeftIconId
        ,@legacyRightIconId
        ,@BoxId
        ,@ShapeIndex
        ,@GraphicIndex
        ,@TextTypeIndex
        ,@OrientationIndex
        ,@effectiveIconId
        ,@effectiveImageId
    )
end
else
begin
    update [Content].[Button]
    set
        Id = @Id
        ,Updated = @now
        ,UpdatedBy = @SessionId
        ,Internal = @Internal
        ,Path = @Path
        ,Text = @effectiveText
        ,TextAlignIndex = null
        ,LeftIconId = @legacyLeftIconId
        ,RightIconId = @legacyRightIconId
        ,BoxId = @BoxId
        ,ShapeIndex = @ShapeIndex
        ,GraphicIndex = @GraphicIndex
        ,TextTypeIndex = @TextTypeIndex
        ,OrientationIndex = @OrientationIndex
        ,IconId = @effectiveIconId
        ,ImageId = @effectiveImageId
    where Id = @Id
end