namespace Crudspa.Content.Design.Server.Sproxies;

public static class ContentPortalUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContentPortal contentPortal)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ContentPortalUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", contentPortal.Id);
        command.AddParameter("@MaxWidth", 10, contentPortal.MaxWidth);
        command.AddParameter("@BrandingImageId", contentPortal.BrandingImageFile.Id);

        await command.Execute(connection, transaction);
    }
}