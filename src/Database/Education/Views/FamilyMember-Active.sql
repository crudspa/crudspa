create view [Education].[FamilyMember-Active] as

select familyMember.Id as Id
    ,familyMember.FamilyId as FamilyId
    ,familyMember.TypeId as TypeId
    ,familyMember.ContactId as ContactId
    ,familyMember.Culture as Culture
    ,familyMember.SmsOptOut as SmsOptOut
    ,familyMember.PhoneChanged as PhoneChanged
    ,familyMember.BookGoal as BookGoal
    ,familyMember.ImportNum as ImportNum
    ,familyMember.SurveyLink as SurveyLink
from [Education].[FamilyMember] familyMember
where 1=1
    and familyMember.IsDeleted = 0
    and familyMember.VersionOf = familyMember.Id