namespace Crudspa.Content.Design.Server.Sproxies;

public static class SectionInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Section section)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SectionInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageId", section.PageId);
        command.AddParameter("@TypeId", section.TypeId);
        command.AddParameter("@BoxId", section.Box.Id);
        command.AddParameter("@ContainerId", section.Container.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}