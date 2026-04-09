create view [Samples].[ComposerContact-Active] as

select composerContact.Id as Id
    ,composerContact.ContactId as ContactId
    ,composerContact.UserId as UserId
from [Samples].[ComposerContact] composerContact
where 1=1
    and composerContact.IsDeleted = 0
    and composerContact.VersionOf = composerContact.Id