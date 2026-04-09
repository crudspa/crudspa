create proc [SamplesComposer].[ComposerContactSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     composerContact.Id
    ,composerContact.UserId
    ,composerContact.ContactId
from [Samples].[ComposerContact-Active] composerContact
    inner join [Framework].[Contact-Active] contact on composerContact.ContactId = contact.Id
    left join [Framework].[User-Active] userTable on composerContact.UserId = userTable.Id
where composerContact.Id = @Id