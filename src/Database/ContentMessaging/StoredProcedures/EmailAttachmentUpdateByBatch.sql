create proc [ContentMessaging].[EmailAttachmentUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@EmailId uniqueidentifier
    ,@PdfId uniqueidentifier
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (
    select 1
    from [Content].[EmailAttachment-Active] attachment
        inner join [Content].[Email-Active] email on attachment.EmailId = email.Id
        inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where attachment.Id = @Id and attachment.EmailId = @EmailId
)
    throw 51000, 'Email attachment access denied.', 1

begin transaction

update [Content].[EmailAttachment]
set
    Id = @Id
    ,PdfId = @PdfId
    ,Ordinal = @Ordinal
where Id = @Id

commit transaction