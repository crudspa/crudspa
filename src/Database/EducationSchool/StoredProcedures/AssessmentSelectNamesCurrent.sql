create proc [EducationSchool].[AssessmentSelectNamesCurrent] as

declare @now datetimeoffset = sysdatetimeoffset()

select
    assessment.Id
    ,assessment.[Name] as Name
from [Education].[Assessment-Active] as assessment
where assessment.AvailableStart <= @now
    and assessment.AvailableEnd >= @now