create proc [ContentMessaging].[EmailSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     email.Id
    ,email.MembershipId
    ,email.Subject
    ,email.FromName
    ,email.FromEmail
    ,email.TemplateId
    ,template.Title as TemplateTitle
    ,email.Send
    ,email.Body
    ,email.Status
    ,email.Processed
    ,message.ActivationId
    ,message.StageId
    ,stage.Name as StageName
from [Content].[Email-Active] email
    inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    left join [Content].[EmailTemplate-Active] template on email.TemplateId = template.Id
    left join [Content].[Message-Active] message on message.EmailId = email.Id
    left join [Content].[Stage-Active] stage on stage.Id = message.StageId
where email.Id = @Id

select
     emailAttachment.Id
    ,emailAttachment.EmailId
    ,email.FromName as EmailFromName
    ,pdf.Id as PdfId
    ,pdf.BlobId as PdfBlobId
    ,pdf.Name as PdfName
    ,pdf.Format as PdfFormat
    ,pdf.Description as PdfDescription
    ,emailAttachment.Ordinal
from [Content].[EmailAttachment-Active] emailAttachment
    inner join [Content].[Email-Active] email on emailAttachment.EmailId = email.Id
    inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
    inner join [Framework].[PdfFile-Active] pdf on emailAttachment.PdfId = pdf.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
where emailAttachment.EmailId = @Id