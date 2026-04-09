create proc [EducationPublisher].[VocabPartDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @assessmentId uniqueidentifier = (select top 1 AssessmentId from [Education].[VocabPart] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[VocabPart-Active] where Id = @Id)
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
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[VocabPart] vocabPart
where vocabPart.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[VocabPart] baseTable
    inner join [Education].[VocabPart-Active] vocabPart on vocabPart.Id = baseTable.Id
where vocabPart.AssessmentId = @assessmentId
    and vocabPart.Ordinal > @oldOrdinal

commit transaction