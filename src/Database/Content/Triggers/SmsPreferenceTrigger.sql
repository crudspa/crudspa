create trigger [Content].[SmsPreferenceTrigger] on [Content].[SmsPreference]
    for update
as

insert [Content].[SmsPreference] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,SmsChannelKey
    ,ContactId
    ,ContactPhoneId
    ,Number
    ,Status
    ,Source
    ,StatusChanged
    ,SmsMessageId
    ,Notes
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PortalId
    ,deleted.SmsChannelKey
    ,deleted.ContactId
    ,deleted.ContactPhoneId
    ,deleted.Number
    ,deleted.Status
    ,deleted.Source
    ,deleted.StatusChanged
    ,deleted.SmsMessageId
    ,deleted.Notes
from deleted