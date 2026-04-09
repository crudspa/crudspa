create proc [EducationPublisher].[ActivityChoiceInsertByBatch] (
     @SessionId uniqueidentifier
    ,@ActivityId uniqueidentifier
    ,@Text nvarchar(max)
    ,@AudioFileId uniqueidentifier
    ,@ImageFileId uniqueidentifier
    ,@IsCorrect bit
    ,@ColumnOrdinal int
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Education].[ActivityChoice] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ActivityId
    ,Text
    ,AudioFileId
    ,ImageFileId
    ,IsCorrect
    ,ColumnOrdinal
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ActivityId
    ,@Text
    ,@AudioFileId
    ,@ImageFileId
    ,@IsCorrect
    ,@ColumnOrdinal
    ,@Ordinal
)

commit transaction