create proc [EducationPublisher].[ListenQuestionDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @listenPartId uniqueidentifier = (select top 1 ListenPartId from [Education].[ListenQuestion] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[ListenQuestion-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Education].[ListenQuestion-Active] listenQuestion
        inner join [Education].[ListenPart-Active] listenPart on listenQuestion.ListenPartId = listenPart.Id
        inner join [Education].[Assessment-Active] assessment on listenPart.AssessmentId = assessment.Id
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    where listenQuestion.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update listenQuestion
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[ListenQuestion] listenQuestion
where listenQuestion.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[ListenQuestion] baseTable
    inner join [Education].[ListenQuestion-Active] listenQuestion on listenQuestion.Id = baseTable.Id
where listenQuestion.ListenPartId = @listenPartId
    and listenQuestion.Ordinal > @oldOrdinal

commit transaction