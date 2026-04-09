create trigger [Education].[FamilyMemberTrigger] on [Education].[FamilyMember]
    for update
as

insert [Education].[FamilyMember] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,FamilyId
    ,TypeId
    ,ContactId
    ,Culture
    ,SmsOptOut
    ,PhoneChanged
    ,BookGoal
    ,ImportNum
    ,SurveyLink
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.FamilyId
    ,deleted.TypeId
    ,deleted.ContactId
    ,deleted.Culture
    ,deleted.SmsOptOut
    ,deleted.PhoneChanged
    ,deleted.BookGoal
    ,deleted.ImportNum
    ,deleted.SurveyLink
from deleted