
namespace Crudspa.Content.Design.Server.Sproxies;

public static class PublicationGroupInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PublicationGroup publicationGroup)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationGroupInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", publicationGroup.PortalId);
        command.AddParameter("@Name", 150, publicationGroup.Name);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}