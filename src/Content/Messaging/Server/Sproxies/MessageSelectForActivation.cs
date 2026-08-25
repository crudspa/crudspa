namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class MessageSelectForActivation
{
    public static async Task<IList<Message>> Execute(String connection, Guid? sessionId, Guid? activationId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.MessageSelectForActivation";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ActivationId", activationId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var messages = new List<Message>();

            while (await reader.ReadAsync())
                messages.Add(ReadMessage(reader));

            return messages;
        });
    }

    private static Message ReadMessage(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            MembershipId = reader.ReadGuid(1),
            StageId = reader.ReadGuid(2),
            StageName = reader.ReadString(3),
            ActivationId = reader.ReadGuid(4),
            EmailId = reader.ReadGuid(5),
            SmsId = reader.ReadGuid(6),
        };
    }
}