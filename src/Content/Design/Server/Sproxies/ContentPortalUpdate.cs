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
        command.AddParameter("@SeoTitle", 100, contentPortal.SeoTitle);
        command.AddParameter("@SeoDescription", 300, contentPortal.SeoDescription);
        command.AddParameter("@SeoKeywords", 300, contentPortal.SeoKeywords);
        command.AddParameter("@SeoImageId", contentPortal.SeoImageFile.Id);
        command.AddParameter("@CanonicalBaseUrl", 250, contentPortal.CanonicalBaseUrl);

        await command.Execute(connection, transaction);
    }
}