
namespace Crudspa.Content.Design.Server.Sproxies;

public static class PublicationResourceTypeSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationResourceTypeSelectOrderables";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadOrderables(connection);
    }
}