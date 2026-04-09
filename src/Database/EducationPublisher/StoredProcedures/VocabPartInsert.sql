create proc [EducationPublisher].[VocabPartInsert] (
     @SessionId uniqueidentifier
    ,@AssessmentId uniqueidentifier
    ,@Title nvarchar(100)
    ,@PreviewInstructionsText nvarchar(max)
    ,@PreviewInstructionsAudioFileId uniqueidentifier
    ,@QuestionsInstructionsText nvarchar(max)
    ,@QuestionsInstructionsAudioFileId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @ordinal int = (select count(1) from [Education].[VocabPart-Active] where AssessmentId = @AssessmentId)

insert [Education].[VocabPart] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,AssessmentId
    ,Title
    ,PreviewInstructionsText
    ,PreviewInstructionsAudioFileId
    ,QuestionsInstructionsText
    ,QuestionsInstructionsAudioFileId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@AssessmentId
    ,@Title
    ,@PreviewInstructionsText
    ,@PreviewInstructionsAudioFileId
    ,@QuestionsInstructionsText
    ,@QuestionsInstructionsAudioFileId
    ,@ordinal
)

if not exists (
    select 1
    from [Education].[VocabPart-Active] vocabPart
        inner join [Education].[Assessment-Active] assessment on vocabPart.AssessmentId = assessment.Id
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    where vocabPart.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction