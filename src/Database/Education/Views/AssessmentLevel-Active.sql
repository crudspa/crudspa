create view [Education].[AssessmentLevel-Active] as

select assessmentLevel.Id as Id
    ,assessmentLevel.[Key] as [Key]
    ,assessmentLevel.Name as Name
    ,assessmentLevel.Ordinal as Ordinal
from [Education].[AssessmentLevel] assessmentLevel
where 1=1
    and assessmentLevel.IsDeleted = 0