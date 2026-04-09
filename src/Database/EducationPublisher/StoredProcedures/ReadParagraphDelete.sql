create proc [EducationPublisher].[ReadParagraphDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @readPartId uniqueidentifier = (select top 1 ReadPartId from [Education].[ReadParagraph] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[ReadParagraph-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Education].[ReadParagraph-Active] readParagraph
        inner join [Education].[ReadPart-Active] readPart on readParagraph.ReadPartId = readPart.Id
        inner join [Education].[Assessment-Active] assessment on readPart.AssessmentId = assessment.Id
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    where readParagraph.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update readParagraph
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[ReadParagraph] readParagraph
where readParagraph.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[ReadParagraph] baseTable
    inner join [Education].[ReadParagraph-Active] readParagraph on readParagraph.Id = baseTable.Id
where readParagraph.ReadPartId = @readPartId
    and readParagraph.Ordinal > @oldOrdinal

commit transaction