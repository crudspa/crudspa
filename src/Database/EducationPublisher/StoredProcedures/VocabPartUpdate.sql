create proc [EducationPublisher].[VocabPartUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(100)
    ,@PreviewInstructionsText nvarchar(max)
    ,@PreviewInstructionsAudioFileId uniqueidentifier
    ,@QuestionsInstructionsText nvarchar(max)
    ,@QuestionsInstructionsAudioFileId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

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

update vocabPart
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,PreviewInstructionsText = @PreviewInstructionsText
    ,PreviewInstructionsAudioFileId = @PreviewInstructionsAudioFileId
    ,QuestionsInstructionsText = @QuestionsInstructionsText
    ,QuestionsInstructionsAudioFileId = @QuestionsInstructionsAudioFileId
from [Education].[VocabPart] vocabPart
where vocabPart.Id = @Id

commit transaction