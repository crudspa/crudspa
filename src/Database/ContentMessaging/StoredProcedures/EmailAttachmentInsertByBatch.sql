create proc [ContentMessaging].[EmailAttachmentInsertByBatch] (
     @SessionId uniqueidentifier
    ,@EmailId uniqueidentifier
    ,@PdfId uniqueidentifier
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (
    select 1
    from [Content].[Email-Active] email
        inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where email.Id = @EmailId
)
    throw 51000, 'Email attachment access denied.', 1

begin transaction

insert [Content].[EmailAttachment] (
     Id
    ,EmailId
    ,PdfId
    ,Ordinal
)
values (
     @Id
    ,@EmailId
    ,@PdfId
    ,@Ordinal
)

commit transaction