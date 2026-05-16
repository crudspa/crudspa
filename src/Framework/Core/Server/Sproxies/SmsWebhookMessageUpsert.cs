namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SmsWebhookMessageUpsert
{
    public static async Task<Guid?> Execute(
        String connection,
        Guid? sessionId,
        String? smsChannelKey,
        Guid? portalId,
        Int32 provider,
        String? providerMessageId,
        String fromNumber,
        String toNumber,
        String? body,
        String? providerStatus,
        Int32? segmentCount)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SmsWebhookMessageUpsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SmsChannelKey", 50, smsChannelKey);
        command.AddParameter("@PortalId", portalId);
        command.AddParameter("@Provider", provider);
        command.AddParameter("@ProviderMessageId", 75, providerMessageId);
        command.AddParameter("@FromNumber", 20, fromNumber);
        command.AddParameter("@ToNumber", 20, toNumber);
        command.AddParameter("@Body", body);
        command.AddParameter("@ProviderStatus", 75, providerStatus);
        command.AddParameter("@SegmentCount", segmentCount);

        var id = command.AddOutputParameter("@Id");

        await command.Execute(connection);

        return (Guid?)id.Value;
    }
}