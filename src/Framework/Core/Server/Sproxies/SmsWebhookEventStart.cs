using System.Data;

namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SmsWebhookEventStart
{
    public static async Task<SmsWebhookEventStartResult> Execute(
        String connection,
        Guid? sessionId,
        String? smsChannelKey,
        Int32 provider,
        Int32 type,
        String idempotencyKey,
        String? providerMessageId,
        String? providerStatus,
        String? requestUrl,
        String? requestSignature,
        Boolean signatureValid,
        String payload,
        Int32 status,
        String? errorMessage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SmsWebhookEventStart";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SmsChannelKey", 50, smsChannelKey);
        command.AddParameter("@Provider", provider);
        command.AddParameter("@Type", type);
        command.AddParameter("@IdempotencyKey", 150, idempotencyKey);
        command.AddParameter("@ProviderMessageId", 75, providerMessageId);
        command.AddParameter("@ProviderStatus", 75, providerStatus);
        command.AddParameter("@RequestUrl", 500, requestUrl);
        command.AddParameter("@RequestSignature", 500, requestSignature);
        command.AddParameter("@SignatureValid", signatureValid);
        command.AddParameter("@Payload", payload);
        command.AddParameter("@Status", status);
        command.AddParameter("@ErrorMessage", errorMessage);

        var id = command.AddOutputParameter("@Id");
        var duplicate = command.AddOutputParameter("@Duplicate", SqlDbType.Bit);

        await command.Execute(connection);

        return new()
        {
            Id = (Guid?)id.Value,
            Duplicate = duplicate.Value is Boolean value && value,
        };
    }
}