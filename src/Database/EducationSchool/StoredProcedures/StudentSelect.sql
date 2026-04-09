create proc [EducationSchool].[StudentSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

select
    student.Id
    ,contact.FirstName
    ,contact.LastName
    ,student.SecretCode
    ,student.GradeId
    ,student.AssessmentLevelGroupId
    ,student.PreferredName
    ,student.AvatarString
    ,student.IdNumber
    ,student.IsTestAccount
    ,family.SchoolId as FamilySchoolId
from [Education].[Student-Active] student
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
where student.Id = @Id
    and family.SchoolId in (select Id from [EducationSchool].[MySchools] (@SessionId))
    and student.DeletedBySchool = 0