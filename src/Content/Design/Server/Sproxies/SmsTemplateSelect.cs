namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsTemplateSelect
{
    public static async Task<SmsTemplate?> Execute(String connection, Guid? sessionId, SmsTemplate smsTemplate)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsTemplateSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", smsTemplate.Id);

        return await command.ReadSingle(connection, ReadSmsTemplate);
    }

    private static SmsTemplate ReadSmsTemplate(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            MembershipId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            Body = reader.ReadString(3),
        };
    }
}