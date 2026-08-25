namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsTemplateSelectFull
{
    public static async Task<IList<SmsTemplateFull>> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsTemplateSelectFull";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ReadAll(connection, ReadSmsTemplate);
    }

    private static SmsTemplateFull ReadSmsTemplate(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            Body = reader.ReadString(3),
        };
    }
}