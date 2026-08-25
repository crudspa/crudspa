namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsPreferenceSelect
{
    public static async Task<SmsPreference?> Execute(String connection, Guid? sessionId, SmsPreference smsPreference)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsPreferenceSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", smsPreference.Id);

        return await command.ReadSingle(connection, ReadSmsPreference);
    }

    private static SmsPreference ReadSmsPreference(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            OrganizationId = reader.ReadGuid(2),
            Number = reader.ReadString(3),
            ContactId = reader.ReadGuid(4),
            ContactFirstName = reader.ReadString(5),
            ContactPhoneId = reader.ReadGuid(6),
            ContactPhonePhone = reader.ReadString(7),
            Status = reader.ReadEnum<SmsPreference.Statuses>(8),
            Source = reader.ReadEnum<SmsPreference.Sources>(9),
            StatusChanged = reader.ReadDateTimeOffset(10),
            Notes = reader.ReadString(11),
        };
    }
}