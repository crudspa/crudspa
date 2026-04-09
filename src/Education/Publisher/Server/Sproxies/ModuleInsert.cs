namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ModuleInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Module module)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ModuleInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BookId", module.BookId);
        command.AddParameter("@Title", 75, module.Title);
        command.AddParameter("@StatusId", module.StatusId);
        command.AddParameter("@IconId", module.IconId);
        command.AddParameter("@RequiresAchievementId", module.RequiresAchievementId);
        command.AddParameter("@GeneratesAchievementId", module.GeneratesAchievementId);
        command.AddParameter("@BinderTypeId", module.Binder.TypeId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}