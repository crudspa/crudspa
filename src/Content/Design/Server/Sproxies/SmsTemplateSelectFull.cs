namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsTemplateSelectFull
{
    public static async Task<IList<SmsTemplateFull>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsTemplateSelectFull";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadSmsTemplate);
    }

    private static SmsTemplateFull ReadSmsTemplate(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            MembershipId = reader.ReadGuid(1),
            MembershipName = reader.ReadString(2),
            Title = reader.ReadString(3),
            Body = reader.ReadString(4),
        };
    }
}