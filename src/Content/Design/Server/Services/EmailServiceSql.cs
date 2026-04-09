using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Server.Services;

public class EmailServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IEmailService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Email>>> SearchForMembership(Request<EmailSearch> request)
    {
        return await wrappers.Try<IList<Email>>(request, async response =>
        {
            var emails = await EmailSelectWhereForMembership.Execute(Connection, request.SessionId, request.Value);

            return emails;
        });
    }

    public async Task<Response<Email?>> Fetch(Request<Email> request)
    {
        return await wrappers.Try<Email?>(request, async response =>
        {
            var email = await EmailSelect.Execute(Connection, request.SessionId, request.Value);

            return email;
        });
    }

    public async Task<Response<Email?>> Add(Request<Email> request)
    {
        return await wrappers.Validate<Email?, Email>(request, async response =>
        {
            var email = request.Value;

            foreach (var emailAttachment in email.EmailAttachments)
            {
                var emailAttachmentPdfFileResponse = await fileService.SavePdf(new(request.SessionId, emailAttachment.PdfFile), emailAttachment.PdfFile.Id);
                if (!emailAttachmentPdfFileResponse.Ok)
                {
                    response.AddErrors(emailAttachmentPdfFileResponse.Errors);
                    return null;
                }

                if (emailAttachmentPdfFileResponse.Value is not null) emailAttachment.PdfFile = emailAttachmentPdfFileResponse.Value;
            }

            email.Body = htmlSanitizer.Sanitize(email.Body);

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var id = await EmailInsert.Execute(connection, transaction, request.SessionId, email);

                foreach (var emailAttachment in email.EmailAttachments)
                {
                    emailAttachment.EmailId = id;
                    await EmailAttachmentInsertByBatch.Execute(connection, transaction, request.SessionId, emailAttachment);
                }

                return new Email
                {
                    Id = id,
                    MembershipId = email.MembershipId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Email> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var email = request.Value;

            var existing = await EmailSelect.Execute(Connection, request.SessionId, email);

            foreach (var emailAttachment in email.EmailAttachments)
            {
                var existingEmailAttachment = existing?.EmailAttachments.FirstOrDefault(x => x.Id.Equals(emailAttachment.Id));

                if (emailAttachment.PdfFile is not null)
                {
                    var emailAttachmentPdfFileResponse = await fileService.SavePdf(new(request.SessionId, emailAttachment.PdfFile), existingEmailAttachment?.PdfFile?.Id);
                    if (!emailAttachmentPdfFileResponse.Ok)
                    {
                        response.AddErrors(emailAttachmentPdfFileResponse.Errors);
                        return;
                    }

                    if (emailAttachmentPdfFileResponse.Value is not null) emailAttachment.PdfFile = emailAttachmentPdfFileResponse.Value;
                }
            }

            email.Body = htmlSanitizer.Sanitize(email.Body);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await EmailUpdate.Execute(connection, transaction, request.SessionId, email);

                await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                    existing!.EmailAttachments,
                    email.EmailAttachments,
                    EmailAttachmentInsertByBatch.Execute,
                    EmailAttachmentUpdateByBatch.Execute,
                    EmailAttachmentDeleteByBatch.Execute);

                email.EmailAttachments.EnsureOrder();
                await EmailAttachmentUpdateOrdinalsByBatch.Execute(connection, transaction, request.SessionId, email.EmailAttachments);
            });
        });
    }

    public async Task<Response> Remove(Request<Email> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var email = request.Value;
            var existing = await EmailSelect.Execute(Connection, request.SessionId, email);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                foreach (var emailAttachment in existing.EmailAttachments)
                    await EmailAttachmentDeleteByBatch.Execute(connection, transaction, request.SessionId, emailAttachment);

                await EmailDelete.Execute(connection, transaction, request.SessionId, email);
            });
        });
    }

    public async Task<Response<IList<EmailTemplateFull>>> FetchEmailTemplates(Request<Membership> request)
    {
        return await wrappers.Try<IList<EmailTemplateFull>>(request, async response =>
            await EmailTemplateSelectFull.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<IList<Token>>> FetchTokens(Request<Membership> request)
    {
        return await wrappers.Try<IList<Token>>(request, async response =>
            await TokenSelectForMembership.Execute(Connection, request.SessionId, request.Value.Id));
    }
}