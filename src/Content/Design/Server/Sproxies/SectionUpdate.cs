namespace Crudspa.Content.Design.Server.Sproxies;

public static class SectionUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Section section)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SectionUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", section.Id);
        command.AddParameter("@TypeId", section.TypeId);
        command.AddParameter("@BoxId", section.Box.Id);
        command.AddParameter("@ContainerId", section.Container.Id);

        await command.Execute(connection, transaction);
    }
}