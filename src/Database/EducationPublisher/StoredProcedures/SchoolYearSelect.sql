create proc [EducationPublisher].[SchoolYearSelect] (
     @Id uniqueidentifier
) as

select
    schoolYear.Id
    ,schoolYear.Name
    ,schoolYear.Starts
    ,schoolYear.Ends
from [Education].[SchoolYear-Active] schoolYear
where schoolYear.Id = @Id