create proc [ContentJobs].[SmsSelectForSending] (
    @SessionId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset(7) = sysdatetimeoffset()
declare @batchId uniqueidentifier = newid()

set nocount on

update sms
set  sms.Updated = @now
    ,sms.UpdatedBy = @SessionId
    ,sms.BatchId = @batchId
    ,sms.Status = 1
    ,sms.Processed = @now
from [Content].[Sms] sms
    inner join [Content].[Membership-Active] membership on sms.MembershipId = membership.Id
    inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
where sms.IsDeleted = 0
    and sms.VersionOf = sms.Id
    and sms.BatchId is null
    and sms.Status = 0
    and sms.Send <= @now
    and portal.OwnerId = @organizationId

select
     sms.Id
    ,sms.MembershipId
    ,sms.SmsChannelKey
    ,membership.PortalId
    ,sms.Body
    ,sms.Send
    ,sms.Status
from [Content].[Sms-Active] sms
    inner join [Content].[Membership-Active] membership on sms.MembershipId = membership.Id
where sms.BatchId = @batchId
order by sms.Send

select
     smsAttachment.Id
    ,smsAttachment.SmsId
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId
    ,imageFile.Name
    ,imageFile.Format
    ,smsAttachment.Ordinal
from [Content].[SmsAttachment-Active] smsAttachment
    inner join [Framework].[ImageFile-Active] imageFile on smsAttachment.ImageId = imageFile.Id
    inner join [Content].[Sms-Active] sms on smsAttachment.SmsId = sms.Id
where sms.BatchId = @batchId
order by smsAttachment.Ordinal