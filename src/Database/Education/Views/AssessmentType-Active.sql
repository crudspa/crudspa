create view [Education].[AssessmentType-Active] as

select assessmentType.Id as Id
    ,assessmentType.[Key] as [Key]
    ,assessmentType.Name as Name
    ,assessmentType.Ordinal as Ordinal
from [Education].[AssessmentType] assessmentType
where 1=1
    and assessmentType.IsDeleted = 0