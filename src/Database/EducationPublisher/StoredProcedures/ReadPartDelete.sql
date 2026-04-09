create proc [EducationPublisher].[ReadPartDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @assessmentId uniqueidentifier = (select top 1 AssessmentId from [Education].[ReadPart] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[ReadPart-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Education].[ReadPart-Active] readPart
        inner join [Education].[Assessment-Active] assessment on readPart.AssessmentId = assessment.Id
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    where readPart.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update readPart
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[ReadPart] readPart
where readPart.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[ReadPart] baseTable
    inner join [Education].[ReadPart-Active] readPart on readPart.Id = baseTable.Id
where readPart.AssessmentId = @assessmentId
    and readPart.Ordinal > @oldOrdinal

commit transaction