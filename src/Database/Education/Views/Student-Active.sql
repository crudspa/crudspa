create view [Education].[Student-Active] as

select student.Id as Id
    ,student.ResearchId as ResearchId
    ,student.ContactId as ContactId
    ,student.UserId as UserId
    ,student.FamilyId as FamilyId
    ,student.StatusId as StatusId
    ,student.SecretCode as SecretCode
    ,student.GenderId as GenderId
    ,student.GradeId as GradeId
    ,student.AssessmentTypeGroupId as AssessmentTypeGroupId
    ,student.AssessmentLevelGroupId as AssessmentLevelGroupId
    ,student.ConditionGroupId as ConditionGroupId
    ,student.GoalSettingGroupId as GoalSettingGroupId
    ,student.PersonalizationGroupId as PersonalizationGroupId
    ,student.ContentGroupId as ContentGroupId
    ,student.AudioGenderId as AudioGenderId
    ,student.ChallengeLevelId as ChallengeLevelId
    ,student.MoreSample as MoreSample
    ,student.TextSample as TextSample
    ,student.PreferredName as PreferredName
    ,student.AvatarGraphicId as AvatarGraphicId
    ,student.AvatarColor as AvatarColor
    ,student.AvatarCharacter as AvatarCharacter
    ,student.AvatarString as AvatarString
    ,student.TermsAccepted as TermsAccepted
    ,student.IdNumber as IdNumber
    ,student.CreatedBySchool as CreatedBySchool
    ,student.DeletedBySchool as DeletedBySchool
    ,student.IsTestAccount as IsTestAccount
    ,student.ResearchGroupId as ResearchGroupId
from [Education].[Student] student
where 1=1
    and student.IsDeleted = 0
    and student.VersionOf = student.Id