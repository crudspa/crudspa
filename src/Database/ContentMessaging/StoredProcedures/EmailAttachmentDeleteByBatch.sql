create proc [ContentMessaging].[EmailAttachmentDeleteByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set xact_abort on
set nocount on

if not exists (
    select 1
    from [Content].[EmailAttachment-Active] attachment
        inner join [Content].[Email-Active] email on attachment.EmailId = email.Id
        inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where attachment.Id = @Id
)
    throw 51000, 'Email attachment access denied.', 1

begin transaction

update [Content].[EmailAttachment]
set  IsDeleted = 1
where Id = @Id

commit transaction