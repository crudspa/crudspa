
namespace Crudspa.Content.Design.Server.Sproxies;

public static class PublicationGroupSelectForPortal
{
    public static async Task<IList<PublicationGroup>> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationGroupSelectForPortal";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var publicationGroups = new List<PublicationGroup>();

            while (await reader.ReadAsync())
                publicationGroups.Add(ReadPublicationGroup(reader));

            return publicationGroups;
        });
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