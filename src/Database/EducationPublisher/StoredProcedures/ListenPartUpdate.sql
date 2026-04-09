create proc [EducationPublisher].[ListenPartUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(100)
    ,@PassageAudioFileId uniqueidentifier
    ,@PassageInstructionsText nvarchar(max)
    ,@PassageInstructionsAudioFileId uniqueidentifier
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
    from [Education].[ListenPart-Active] listenPart
        inner join [Education].[Assessment-Active] assessment on listenPart.AssessmentId = assessment.Id
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    where listenPart.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update listenPart
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,PassageAudioFileId = @PassageAudioFileId
    ,PassageInstructionsText = @PassageInstructionsText
    ,PassageInstructionsAudioFileId = @PassageInstructionsAudioFileId
    ,PreviewInstructionsText = @PreviewInstructionsText
    ,PreviewInstructionsAudioFileId = @PreviewInstructionsAudioFileId
    ,QuestionsInstructionsText = @QuestionsInstructionsText
    ,QuestionsInstructionsAudioFileId = @QuestionsInstructionsAudioFileId
from [Education].[ListenPart] listenPart
where listenPart.Id = @Id

commit transaction