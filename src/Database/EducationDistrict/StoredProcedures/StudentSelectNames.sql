create proc [EducationDistrict].[StudentSelectNames] (
     @SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @schoolYearId uniqueidentifier = (select top 1 Id from [Education].[SchoolYear] where Starts <= @now and Ends > @now order by Starts desc)

declare @mySchools table (Id uniqueidentifier)
insert into @mySchools (Id) select Id from [EducationDistrict].[MySchools] (@SessionId)

select
     student.Id
    ,trim(contact.FirstName + ' ' + contact.LastName) as Name
from [Education].[Student-Active] as student
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    inner join @mySchools school on family.SchoolId = school.Id
where family.SchoolId in (select Id from [EducationDistrict].[MySchools] (@SessionId))
    and student.DeletedBySchool = 0
    and student.Id in (select StudentId from [Education].[StudentSchoolYear-Active] where SchoolYearId = @schoolYearId)
order by contact.FirstName, contact.LastName