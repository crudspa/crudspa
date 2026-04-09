namespace Crudspa.Framework.Core.Server.Services;

public class AccessCodeServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService,
    ICryptographyService cryptographyService,
    IEmailSender emailSender,
    IEmailLayoutService emailLayoutService)
    : IAccessCodeService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response> Generate(Request<User> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var userId = request.Value.Id;

            var user = await UserSelect.Execute(Connection, userId);

            if (user is null)
            {
                response.Errors.Add(new() { Message = $"User '{userId:D}' not found." });
                return;
            }

            if (!user.Username.IsEmailAddress() && user.Contact.Emails.HasNothing())
            {
                response.AddError($"No associated email addresses for user '{userId:D}' could be found.");
                return;
            }

            var address = user.Username.IsEmailAddress()
                ? user.Username
                : user.Contact.Emails.OrderBy(x => x.Ordinal).ThenBy(x => x.Email).First().Email;

            var portal = await PortalRunSelect.Execute(Connection, user.PortalId);

            var accessCode = new AccessCode
            {
                PortalId = user.PortalId,
                Contact = user.Contact,
                Username = user.Username,
                Code = cryptographyService.GetRandomInt(0, 999999).ToString("000000"),
                PortalName = portal!.Title,
                EmailAddress = address,
            };

            await AccessCodeInsert.Execute(Connection, request.SessionId, user.Id, accessCode);

            var email = await BuildEmail(accessCode);

            var sendResponse = await emailSender.Send(new(request.SessionId, email));

            if (!sendResponse.Ok)
                response.AddErrors(sendResponse.Errors);
        });
    }

    private async Task<EmailMessage> BuildEmail(AccessCode accessCode)
    {
        var config = configService.Fetch();

        var layout = await emailLayoutService.Fetch(Constants.EmailLayoutKeys.AccessCode);

        var body = $"""
                    <h3>Access Code for {accessCode.PortalName}</h3>
                    <p><br></p>
                    <p>Below is an access code for {accessCode.PortalName}.</p>
                    <p><br></p>
                    <p>Username: <strong>{accessCode.Username}</strong></p>
                    <p>Access Code: <strong>{accessCode.Code}</strong></p>
                    <p><br></p>
                    <p>This code can only be used once and expires in 10 minutes.</p>
                    """;

        return new()
        {
            From = new()
            {
                Name = config.EmailFromName,
                Address = config.EmailFromAddress,
            },
            Subject = $"{accessCode.Code} | Access Code for {accessCode.PortalName}",
            Message = layout.Replace("[Body]", body),
            Private = true,
            To =
            [
                new()
                {
                    Address = accessCode.EmailAddress,
                    Name = accessCode.Contact!.Name,
                },
            ],
        };
    }
}