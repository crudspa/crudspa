create view [Education].[Assessment-Active] as

select assessment.Id as Id
    ,assessment.OwnerId as OwnerId
    ,assessment.Name as Name
    ,assessment.StatusId as StatusId
    ,assessment.AvailableStart as AvailableStart
    ,assessment.AvailableEnd as AvailableEnd
    ,assessment.GradeId as GradeId
    ,assessment.CategoryId as CategoryId
    ,assessment.ImageFileId as ImageFileId
from [Education].[Assessment] assessment
where 1=1
    and assessment.IsDeleted = 0
    and assessment.VersionOf = assessment.Id