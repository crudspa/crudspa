create proc [EducationPublisher].[VocabQuestionDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @vocabPartId uniqueidentifier = (select top 1 VocabPartId from [Education].[VocabQuestion] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[VocabQuestion-Active] where Id = @Id)
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
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[VocabQuestion] vocabQuestion
where vocabQuestion.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[VocabQuestion] baseTable
    inner join [Education].[VocabQuestion-Active] vocabQuestion on vocabQuestion.Id = baseTable.Id
where vocabQuestion.VocabPartId = @vocabPartId
    and vocabQuestion.Ordinal > @oldOrdinal

commit transaction