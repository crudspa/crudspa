create proc [EducationPublisher].[ListenPartDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @assessmentId uniqueidentifier = (select top 1 AssessmentId from [Education].[ListenPart] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[ListenPart-Active] where Id = @Id)
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
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[ListenPart] listenPart
where listenPart.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[ListenPart] baseTable
    inner join [Education].[ListenPart-Active] listenPart on listenPart.Id = baseTable.Id
where listenPart.AssessmentId = @assessmentId
    and listenPart.Ordinal > @oldOrdinal

commit transaction