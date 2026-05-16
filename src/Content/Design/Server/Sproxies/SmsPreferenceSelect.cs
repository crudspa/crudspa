namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsPreferenceSelect
{
    public static async Task<SmsPreference?> Execute(String connection, Guid? sessionId, SmsPreference smsPreference)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsPreferenceSelect";

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
            Number = reader.ReadString(2),
            ContactId = reader.ReadGuid(3),
            ContactFirstName = reader.ReadString(4),
            ContactPhoneId = reader.ReadGuid(5),
            ContactPhonePhone = reader.ReadString(6),
            Status = reader.ReadEnum<SmsPreference.Statuses>(7),
            Source = reader.ReadEnum<SmsPreference.Sources>(8),
            StatusChanged = reader.ReadDateTimeOffset(9),
            Notes = reader.ReadString(10),
        };
    }
}