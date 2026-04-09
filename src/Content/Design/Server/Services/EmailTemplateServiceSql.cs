namespace Crudspa.Content.Design.Server.Services;

public class EmailTemplateServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IHtmlSanitizer htmlSanitizer)
    : IEmailTemplateService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<EmailTemplate>>> SearchForMembership(Request<EmailTemplateSearch> request)
    {
        return await wrappers.Try<IList<EmailTemplate>>(request, async response =>
        {
            var emailTemplates = await EmailTemplateSelectWhereForMembership.Execute(Connection, request.SessionId, request.Value);

            return emailTemplates;
        });
    }

    public async Task<Response<EmailTemplate?>> Fetch(Request<EmailTemplate> request)
    {
        return await wrappers.Try<EmailTemplate?>(request, async response =>
        {
            var emailTemplate = await EmailTemplateSelect.Execute(Connection, request.SessionId, request.Value);

            return emailTemplate;
        });
    }

    public async Task<Response<EmailTemplate?>> Add(Request<EmailTemplate> request)
    {
        return await wrappers.Validate<EmailTemplate?, EmailTemplate>(request, async response =>
        {
            var emailTemplate = request.Value;

            emailTemplate.Body = htmlSanitizer.Sanitize(emailTemplate.Body);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await EmailTemplateInsert.Execute(connection, transaction, request.SessionId, emailTemplate);

                return new EmailTemplate
                {
                    Id = id,
                    MembershipId = emailTemplate.MembershipId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<EmailTemplate> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var emailTemplate = request.Value;

            emailTemplate.Body = htmlSanitizer.Sanitize(emailTemplate.Body);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await EmailTemplateUpdate.Execute(connection, transaction, request.SessionId, emailTemplate);
            });
        });
    }

    public async Task<Response> Remove(Request<EmailTemplate> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var emailTemplate = request.Value;
            var existing = await EmailTemplateSelect.Execute(Connection, request.SessionId, emailTemplate);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await EmailTemplateDelete.Execute(connection, transaction, request.SessionId, emailTemplate);
            });
        });
    }

    public async Task<Response<IList<Token>>> FetchTokens(Request<Membership> request)
    {
        return await wrappers.Try<IList<Token>>(request, async response =>
            await TokenSelectForMembership.Execute(Connection, request.SessionId, request.Value.Id));
    }
}