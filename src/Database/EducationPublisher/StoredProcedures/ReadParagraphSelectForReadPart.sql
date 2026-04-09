create proc [EducationPublisher].[ReadParagraphSelectForReadPart] (
     @SessionId uniqueidentifier
    ,@ReadPartId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     readParagraph.Id
    ,readParagraph.ReadPartId
    ,readPart.Title as ReadPartTitle
    ,readParagraph.Text
    ,readParagraph.Ordinal
from [Education].[ReadParagraph-Active] readParagraph
    inner join [Education].[ReadPart-Active] readPart on readParagraph.ReadPartId = readPart.Id
    inner join [Education].[Assessment-Active] assessment on readPart.AssessmentId = assessment.Id
    inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
where readParagraph.ReadPartId = @ReadPartId
    and organization.Id = @organizationId