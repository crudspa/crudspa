namespace Crudspa.Content.Design.Server.Sproxies;

using Publication = Shared.Contracts.Data.Publication;

public static class PublicationInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Publication publication)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", publication.PortalId);
        command.AddParameter("@GroupId", publication.GroupId);
        command.AddParameter("@StatusId", publication.StatusId);
        command.AddParameter("@Title", 300, publication.Title);
        command.AddParameter("@Citation", publication.Citation);
        command.AddParameter("@Venue", 250, publication.Venue);
        command.AddParameter("@Sample", 500, publication.Sample);
        command.AddParameter("@Timeline", 100, publication.Timeline);
        command.AddParameter("@Outcome", 1000, publication.Outcome);
        command.AddParameter("@Intervention", 1000, publication.Intervention);
        command.AddParameter("@ImageId", publication.ImageFile.Id);
        command.AddParameter("@ImageAlt", 300, publication.ImageAlt);
        command.AddParameter("@Year", publication.Year);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}