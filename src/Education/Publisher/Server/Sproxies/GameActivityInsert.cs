namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameActivityInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, GameActivity gameActivity)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameActivityInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SectionId", gameActivity.SectionId);
        command.AddParameter("@ThemeWord", 50, gameActivity.ThemeWord);
        command.AddParameter("@GroupId", gameActivity.GroupId);
        command.AddParameter("@TypeId", gameActivity.TypeId);
        command.AddParameter("@Rigorous", gameActivity.Rigorous ?? false);
        command.AddParameter("@Multisyllabic", gameActivity.Multisyllabic ?? false);
        command.AddParameter("@ActivityId", gameActivity.ActivityId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}