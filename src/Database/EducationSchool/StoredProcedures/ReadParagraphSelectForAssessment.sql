create proc [EducationSchool].[ReadParagraphSelectForAssessment] (
     @AssessmentId uniqueidentifier
) as

select
    readParagraph.Id as Id
    ,readParagraph.ReadPartId as ReadPartId
    ,readParagraph.Text as Text
    ,readParagraph.Ordinal as Ordinal
from [Education].[ReadParagraph-Active] readParagraph
    inner join [Education].[ReadPart] readPart on readParagraph.ReadPartId = readPart.Id
    inner join [Education].[Assessment] assessment on readPart.AssessmentId = assessment.Id
where assessment.Id = @AssessmentId