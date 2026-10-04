
namespace Crudspa.Content.Design.Server.Sproxies;

public static class PublicationGroupSelect
{
    public static async Task<PublicationGroup?> Execute(String connection, Guid? sessionId, PublicationGroup publicationGroup)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationGroupSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publicationGroup.Id);

        return await command.ReadSingle(connection, ReadPublicationGroup);
    }

    private static PublicationGroup ReadPublicationGroup(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            Name = reader.ReadString(2),
            Ordinal = reader.ReadInt32(3),
        };
    }
}