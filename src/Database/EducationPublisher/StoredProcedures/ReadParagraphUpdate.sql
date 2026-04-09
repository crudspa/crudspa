create proc [EducationPublisher].[ReadParagraphUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Text nvarchar(max)
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
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Text = @Text
from [Education].[ReadParagraph] readParagraph
where readParagraph.Id = @Id

commit transaction