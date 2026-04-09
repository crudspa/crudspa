namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailTemplateSelectFull
{
    public static async Task<IList<EmailTemplateFull>> Execute(String connection, Guid? sessionId, Guid? membershipId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailTemplateSelectFull";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MembershipId", membershipId);

        return await command.ReadAll(connection, ReadEmailTemplate);
    }

    private static EmailTemplateFull ReadEmailTemplate(SqlDataReader reader)
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