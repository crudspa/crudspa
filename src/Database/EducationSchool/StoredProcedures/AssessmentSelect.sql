create proc [EducationSchool].[AssessmentSelect] (
     @Id uniqueidentifier
) as

select
    assessment.Id as Id
    ,assessment.Name as Name
    ,assessment.StatusId as StatusId
    ,assessment.AvailableStart as AvailableStart
    ,assessment.AvailableEnd as AvailableEnd
from [Education].[Assessment-Active] assessment
where assessment.Id = @Id