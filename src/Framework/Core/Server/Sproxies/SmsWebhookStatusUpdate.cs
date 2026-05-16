namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SmsWebhookStatusUpdate
{
    public static async Task<Guid?> Execute(
        String connection,
        Guid? sessionId,
        String? smsChannelKey,
        Int32 provider,
        String? providerMessageId,
        String? providerStatus,
        Int32 status,
        String? providerErrorCode,
        String? providerErrorMessage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SmsWebhookStatusUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SmsChannelKey", 50, smsChannelKey);
        command.AddParameter("@Provider", provider);
        command.AddParameter("@ProviderMessageId", 75, providerMessageId);
        command.AddParameter("@ProviderStatus", 75, providerStatus);
        command.AddParameter("@Status", status);
        command.AddParameter("@ProviderErrorCode", 25, providerErrorCode);
        command.AddParameter("@ProviderErrorMessage", 500, providerErrorMessage);

        var id = command.AddOutputParameter("@Id");

        await command.Execute(connection);

        return id.Value is DBNull ? null : (Guid?)id.Value;
    }
}