using Crudspa.Content.Jobs.Shared.Contracts.Config;

namespace Crudspa.Content.Jobs.Server.Actions;

public class SendEmails(
    ILogger<SendEmails> logger,
    IServerConfigService serverConfig,
    IBlobService blobService,
    IEmailLayoutService emailLayoutService,
    IEmailSender emailSender,
    IContentActionService contentActionService)
    : IJobAction
{
    public SendEmailsConfig? Config { get; set; }
    public ServerConfig? ServerConfig { get; set; }

    private Guid? _sessionId;

    public void Configure(Guid? sessionId, String json)
    {
        _sessionId = sessionId;

        Config = json.FromJson<SendEmailsConfig>();
        ServerConfig = serverConfig.Fetch();
    }

    public async Task<Boolean> Run(Guid? jobId)
    {
        try
        {
            if (Config is null)
                throw new("Config is null.");

            var errors = Config.Validate();

            if (errors.HasItems())
                throw new("Config is invalid. " + errors.ToStringWithSpaces());

            logger.LogInformation("Fetching scheduled emails...");

            var emailsResponse = await contentActionService.FetchEmailsForSending(new(_sessionId));

            if (!emailsResponse.Ok)
                throw new("Call to IFrameworkActionService.FetchEmailsForSending() failed. " + emailsResponse.ErrorMessages);

            var emails = emailsResponse.Value;

            logger.LogInformation("Found {emailCount} emails to send.", emails.Count);

            foreach (var email in emails)
            {
                logger.LogInformation("Processing email {emailId}: '{emailSubject}' ({attachmentCount} attachments)...", email.Id, email.Subject, email.EmailAttachments.Count);

                var layout = await emailLayoutService.Fetch(Constants.EmailLayoutKeys.Message);

                logger.LogInformation("Fetching members...");

                var membersResponse = await contentActionService.FetchMembers(new(_sessionId, new() { Id = email.MembershipId }));

                if (!membersResponse.Ok)
                    throw new("Call to IFrameworkActionService.FetchMembers() failed. " + membersResponse.ErrorMessages);

                var members = membersResponse.Value;

                logger.LogInformation("Found {memberCount} members.", members.Count);

                try
                {
                    var succeeded = 0;
                    var failed = 0;

                    foreach (var member in members)
                    {
                        logger.LogInformation("Composing email message to {contactName} ({contactEmail})...", member.Contact.Name, member.Contact.Email);

                        var subject = ReplaceTokens(email.Subject, member.TokenValues);
                        var body = ReplaceTokens(email.Body, member.TokenValues);

                        var emailMessage = new EmailMessage
                        {
                            From = new()
                            {
                                Name = email.FromName,
                                Address = ServerConfig?.EmailFromAddress,
                            },
                            ReplyTo = new()
                            {
                                Name = email.FromName,
                                Address = email.FromEmail,
                            },
                            To =
                            [
                                new()
                                {
                                    Name = member.Contact.Name,
                                    Address = member.Contact.Email,
                                },
                            ],
                            Subject = subject,
                            Message = layout.Replace("[Body]", body),
                        };

                        foreach (var attachment in email.EmailAttachments)
                        {
                            var blob = await blobService.Fetch(new() { Id = attachment.PdfFile.BlobId });

                            if (blob?.Data is not null)
                                emailMessage.Attachments.Add(new()
                                {
                                    Name = attachment.PdfFile.Name,
                                    Data = blob.Data,
                                });
                        }

                        logger.LogInformation("Sending email '{subject}' to {contactEmail}...", subject, member.Contact.Email);

                        var sendResponse = await emailSender.Send(new(emailMessage));

                        var emailLog = new EmailLog
                        {
                            EmailId = email.Id,
                            RecipientId = member.Contact.Id,
                            RecipientEmail = member.Contact.Email,
                        };

                        if (sendResponse.Ok)
                        {
                            succeeded++;
                            emailLog.Status = EmailLog.Statuses.Succeeded;
                        }
                        else
                        {
                            failed++;
                            emailLog.Status = EmailLog.Statuses.Failed;
                            emailLog.ApiResponse = sendResponse.ErrorMessages;
                        }

                        logger.LogInformation("Writing status {status} to email log for email {emailId}...", emailLog.Status, emailLog.EmailId);

                        await contentActionService.SaveLog(new(_sessionId, emailLog));
                    }

                    logger.LogInformation("Email action complete. Succeeded: {succeeded} | Failed: {failed}", succeeded, failed);

                    email.Status = Email.Statuses.Sent;
                }
                catch (Exception ex)
                {
                    email.Status = Email.Statuses.Failed;
                    logger.LogError(ex, "Unexpected error while sending messages.");
                }
                finally
                {
                    logger.LogInformation("Updating status {status} of email {emailId}...", email.Status, email.Id);
                    await contentActionService.UpdateStatus(new(_sessionId, email));
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while sending bulk messages.");
            return false;
        }
    }

    public String? ReplaceTokens(String? input, IEnumerable<TokenValue> tokenValues)
    {
        if (input.HasNothing())
            return null;

        var output = input;

        foreach (var tokenValue in tokenValues)
            output = output.Replace($"[{tokenValue.TokenKey}]", $"{tokenValue.Value}");

        return output;
    }
}