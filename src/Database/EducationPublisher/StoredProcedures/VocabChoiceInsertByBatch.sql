create proc [EducationPublisher].[VocabChoiceInsertByBatch] (
     @SessionId uniqueidentifier
    ,@VocabQuestionId uniqueidentifier
    ,@Word nvarchar(50)
    ,@IsCorrect bit
    ,@AudioFileId uniqueidentifier
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Education].[VocabChoice] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,VocabQuestionId
    ,Word
    ,IsCorrect
    ,AudioFileId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@VocabQuestionId
    ,@Word
    ,@IsCorrect
    ,@AudioFileId
    ,@Ordinal
)

commit transaction