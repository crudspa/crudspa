create proc [EducationPublisher].[ListenChoiceInsertByBatch] (
     @SessionId uniqueidentifier
    ,@ListenQuestionId uniqueidentifier
    ,@Text nvarchar(max)
    ,@IsCorrect bit
    ,@ImageFileId uniqueidentifier
    ,@AudioFileId uniqueidentifier
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Education].[ListenChoice] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ListenQuestionId
    ,Text
    ,IsCorrect
    ,ImageFileId
    ,AudioFileId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ListenQuestionId
    ,@Text
    ,@IsCorrect
    ,@ImageFileId
    ,@AudioFileId
    ,@Ordinal
)

commit transaction