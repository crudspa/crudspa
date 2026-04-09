create proc [EducationPublisher].[ActivityChoiceUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@ActivityId uniqueidentifier
    ,@Text nvarchar(max)
    ,@AudioFileId uniqueidentifier
    ,@ImageFileId uniqueidentifier
    ,@IsCorrect bit
    ,@ColumnOrdinal int
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Education].[ActivityChoice]
set
    Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Text = @Text
    ,AudioFileId = @AudioFileId
    ,ImageFileId = @ImageFileId
    ,IsCorrect = @IsCorrect
    ,ColumnOrdinal = @ColumnOrdinal
    ,Ordinal = @Ordinal
where Id = @Id

commit transaction