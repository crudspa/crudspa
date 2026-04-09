create proc [EducationPublisher].[ReadQuestionDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @readPartId uniqueidentifier = (select top 1 ReadPartId from [Education].[ReadQuestion] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[ReadQuestion-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Education].[ReadQuestion-Active] readQuestion
        inner join [Education].[ReadPart-Active] readPart on readQuestion.ReadPartId = readPart.Id
        inner join [Education].[Assessment-Active] assessment on readPart.AssessmentId = assessment.Id
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    where readQuestion.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update readQuestion
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[ReadQuestion] readQuestion
where readQuestion.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[ReadQuestion] baseTable
    inner join [Education].[ReadQuestion-Active] readQuestion on readQuestion.Id = baseTable.Id
where readQuestion.ReadPartId = @readPartId
    and readQuestion.Ordinal > @oldOrdinal

commit transaction