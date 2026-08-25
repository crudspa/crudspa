namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class EmailTemplateSelect
{
    public static async Task<EmailTemplate?> Execute(String connection, Guid? sessionId, EmailTemplate emailTemplate)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.EmailTemplateSelect";

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
            PortalId = reader.ReadGuid(2),
            OrganizationId = reader.ReadGuid(3),
            Title = reader.ReadString(4),
            Subject = reader.ReadString(5),
            Body = reader.ReadString(6),
        };
    }
}