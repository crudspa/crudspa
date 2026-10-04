
namespace Crudspa.Content.Design.Server.Sproxies;

public static class PublicationResourceUpdateByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PublicationResource publicationResource)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationResourceUpdateByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publicationResource.Id);
        command.AddParameter("@PublicationId", publicationResource.PublicationId);
        command.AddParameter("@TypeId", publicationResource.TypeId);
        command.AddParameter("@Label", 150, publicationResource.Label);
        command.AddParameter("@Url", 2000, publicationResource.Url);
        command.AddParameter("@PdfId", publicationResource.PdfFile.Id);
        command.AddParameter("@ImageId", publicationResource.ImageFile.Id);
        command.AddParameter("@Ordinal", publicationResource.Ordinal);

        await command.Execute(connection, transaction);
    }
}