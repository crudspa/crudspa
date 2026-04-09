create view [Education].[SchoolContact-Active] as

select schoolContact.Id as Id
    ,schoolContact.SchoolId as SchoolId
    ,schoolContact.ContactId as ContactId
    ,schoolContact.UserId as UserId
    ,schoolContact.TitleId as TitleId
    ,schoolContact.TestAccount as TestAccount
    ,schoolContact.Treatment as Treatment
    ,schoolContact.IdNumber as IdNumber
    ,schoolContact.ResearchGroupId as ResearchGroupId
from [Education].[SchoolContact] schoolContact
where 1=1
    and schoolContact.IsDeleted = 0
    and schoolContact.VersionOf = schoolContact.Id