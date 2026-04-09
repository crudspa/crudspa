create proc [ContentJobs].[EmailSelectForSending] (
     @SessionId uniqueidentifier
) as

declare @now datetimeoffset(7) = sysdatetimeoffset()
declare @batchId uniqueidentifier = newid()

set nocount on

update email
set  email.Updated = @now
    ,email.UpdatedBy = @SessionId
    ,email.BatchId = @batchId
    ,email.Status = 1
    ,email.Processed = @now
from [Content].[Email] email
    inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
where email.IsDeleted = 0
    and email.VersionOf = email.Id
    and email.BatchId is null
    and email.Status = 0
    and email.Send <= @now

select
     email.Id
    ,email.MembershipId
    ,email.Subject
    ,email.FromName
    ,email.FromEmail
    ,email.Send
    ,email.Body
    ,email.Status
    ,email.Processed
from [Content].[Email-Active] email
    inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
where email.BatchId = @batchId

select
     emailAttachment.Id
    ,emailAttachment.EmailId
    ,pdf.Id as PdfId
    ,pdf.BlobId as PdfBlobId
    ,pdf.Name as PdfName
    ,pdf.Format as PdfFormat
    ,pdf.Description as PdfDescription
    ,emailAttachment.Ordinal
from [Content].[EmailAttachment-Active] emailAttachment
    inner join [Content].[Email-Active] email on emailAttachment.EmailId = email.Id
    inner join [Framework].[PdfFile-Active] pdf on emailAttachment.PdfId = pdf.Id
where email.BatchId = @batchId