create proc [EducationPublisher].[VocabQuestionUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Word nvarchar(50)
    ,@AudioFileId uniqueidentifier
    ,@IsPreview bit
    ,@PageBreakBefore bit
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
    from [Education].[VocabQuestion-Active] vocabQuestion
        inner join [Education].[VocabPart-Active] vocabPart on vocabQuestion.VocabPartId = vocabPart.Id
        inner join [Education].[Assessment-Active] assessment on vocabPart.AssessmentId = assessment.Id
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    where vocabQuestion.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update vocabQuestion
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Word = @Word
    ,AudioFileId = @AudioFileId
    ,IsPreview = @IsPreview
    ,PageBreakBefore = @PageBreakBefore
from [Education].[VocabQuestion] vocabQuestion
where vocabQuestion.Id = @Id

commit transaction