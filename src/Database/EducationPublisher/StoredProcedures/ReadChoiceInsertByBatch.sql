create proc [EducationPublisher].[ReadChoiceInsertByBatch] (
     @SessionId uniqueidentifier
    ,@ReadQuestionId uniqueidentifier
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

insert [Education].[ReadChoice] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ReadQuestionId
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
    ,@ReadQuestionId
    ,@Text
    ,@IsCorrect
    ,@ImageFileId
    ,@AudioFileId
    ,@Ordinal
)

commit transaction