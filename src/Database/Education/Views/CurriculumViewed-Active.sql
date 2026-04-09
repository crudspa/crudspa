create view [Education].[CurriculumViewed-Active] as

select curriculumViewed.Id as Id
from [Education].[CurriculumViewed] curriculumViewed
where 1=1
    and curriculumViewed.IsDeleted = 0