namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailTemplateSelect
{
    public static async Task<EmailTemplate?> Execute(String connection, Guid? sessionId, EmailTemplate emailTemplate)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailTemplateSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", emailTemplate.Id);

        return await command.ReadSingle(connection, ReadEmailTemplate);
    }

    private static EmailTemplate ReadEmailTemplate(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            MembershipId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            Subject = reader.ReadString(3),
            Body = reader.ReadString(4),
        };
    }
}