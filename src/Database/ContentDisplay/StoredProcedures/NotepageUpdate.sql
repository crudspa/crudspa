create proc [ContentDisplay].[NotepageUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Text nvarchar(max)
    ,@SelectedImageFileId uniqueidentifier
) as
begin transaction

    declare @now datetimeoffset = sysdatetimeoffset()

    update [Content].[Notepage]
    set
        Updated = @now
        ,UpdatedBy = @SessionId
        ,Text = @Text
        ,SelectedImageFileId = @SelectedImageFileId
    where Id = @Id

commit transaction