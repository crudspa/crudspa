namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsMessageSelect
{
    public static async Task<SmsMessage?> Execute(String connection, Guid? sessionId, SmsMessage smsMessage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsMessageSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", smsMessage.Id);

        return await command.ReadSingle(connection, ReadSmsMessage);
    }

    public static SmsMessage ReadSmsMessage(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            MembershipId = reader.ReadGuid(1),
            PortalId = reader.ReadGuid(2),
            SmsId = reader.ReadGuid(3),
            SmsChannelKey = reader.ReadString(4),
            MemberId = reader.ReadGuid(5),
            Body = reader.ReadString(6),
            Direction = reader.ReadEnum<SmsMessage.Directions>(7),
            Occurred = reader.ReadDateTimeOffset(8),
            FromNumber = reader.ReadString(9),
            ToNumber = reader.ReadString(10),
            Status = reader.ReadEnum<SmsMessage.Statuses>(11),
            ProviderMessageId = reader.ReadString(12),
            Provider = reader.ReadEnum<SmsMessage.Providers>(13),
            ApiResponse = reader.ReadString(14),
            ContactPhoneId = reader.ReadGuid(15),
            ContactId = reader.ReadGuid(16),
            ContactFirstName = reader.ReadString(17),
            ContactLastName = reader.ReadString(18),
        };
    }
}