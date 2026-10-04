
namespace Crudspa.Content.Design.Server.Sproxies;

public static class PublicationGroupSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationGroupSelectOrderables";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadOrderables(connection);
    }
}