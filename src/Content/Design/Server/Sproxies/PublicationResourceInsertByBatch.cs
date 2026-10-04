
namespace Crudspa.Content.Design.Server.Sproxies;

public static class PublicationResourceInsertByBatch
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PublicationResource publicationResource)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationResourceInsertByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PublicationId", publicationResource.PublicationId);
        command.AddParameter("@TypeId", publicationResource.TypeId);
        command.AddParameter("@Label", 150, publicationResource.Label);
        command.AddParameter("@Url", 2000, publicationResource.Url);
        command.AddParameter("@PdfId", publicationResource.PdfFile.Id);
        command.AddParameter("@ImageId", publicationResource.ImageFile.Id);
        command.AddParameter("@Ordinal", publicationResource.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}