namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class TrifoldInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Trifold trifold)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.TrifoldInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BookId", trifold.BookId);
        command.AddParameter("@StatusId", trifold.StatusId);
        command.AddParameter("@RequiresAchievementId", trifold.RequiresAchievementId);
        command.AddParameter("@GeneratesAchievementId", trifold.GeneratesAchievementId);
        command.AddParameter("@BinderTypeId", trifold.Binder.TypeId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}