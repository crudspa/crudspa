create proc [EducationStudent].[StudentSelectBySession] (
     @SessionId uniqueidentifier
) as

select
    student.Id
    ,student.ResearchId
    ,student.ContactId
    ,convert(bit, case when users.Id is null then 0 else 1 end) as MaySignIn
    ,contact.FirstName
    ,contact.LastName
    ,student.FamilyId
    ,student.StatusId
    ,contactEmail.Email as Email
    ,student.GenderId
    ,student.GradeId
    ,student.AssessmentTypeGroupId
    ,student.AssessmentLevelGroupId
    ,student.ConditionGroupId
    ,student.GoalSettingGroupId
    ,student.PersonalizationGroupId
    ,student.ContentGroupId
    ,student.AudioGenderId
    ,student.ChallengeLevelId
    ,student.PreferredName
    ,student.AvatarString
    ,student.ResearchGroupId
    ,audioGender.Name as AudioGenderName
    ,challengeLevel.Name as ChallengeLevelName
    ,family.SchoolId as FamilySchoolId
    ,familySchool.[Key] as FamilySchoolKey
    ,familySchoolOrganization.Name as FamilySchoolOrganizationName
    ,familySchoolDistrictOrganization.Name as FamilySchoolDistrictOrganizationName
    ,grade.Name as GradeName
    ,status.Name as StatusName
    ,contact.TimeZoneId as ContactTimeZoneId
    ,(select count(1) from [Education].[StudentBook-Active] studentBook where studentBook.StudentId = student.Id) as StudentBookCount
from [Education].[Student-Active] student
    inner join [Education].[AudioGender-Active] audioGender on student.AudioGenderId = audioGender.Id
    inner join [Education].[ChallengeLevel-Active] challengeLevel on student.ChallengeLevelId = challengeLevel.Id
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Education].[School-Active] familySchool on family.SchoolId = familySchool.Id
    inner join [Framework].[Organization-Active] familySchoolOrganization on familySchool.OrganizationId = familySchoolOrganization.Id
    inner join [Education].[District-Active] familySchoolDistrict on familySchool.DistrictId = familySchoolDistrict.Id
    inner join [Framework].[Organization-Active] familySchoolDistrictOrganization on familySchoolDistrict.OrganizationId = familySchoolDistrictOrganization.Id
    inner join [Education].[Grade-Active] grade on student.GradeId = grade.Id
    inner join [Education].[StudentStatus-Active] status on student.StatusId = status.Id
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    inner join [Framework].[User-Active] users on users.ContactId = contact.Id
    inner join [Framework].[Session-Active] session on session.UserId = users.Id
    left join [Framework].[ContactEmail] contactEmail on contactEmail.ContactId = contact.Id
where session.Id = @SessionId