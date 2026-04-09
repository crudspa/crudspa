namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ModuleUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Module module)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ModuleUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", module.Id);
        command.AddParameter("@Title", 75, module.Title);
        command.AddParameter("@StatusId", module.StatusId);
        command.AddParameter("@IconId", module.IconId);
        command.AddParameter("@RequiresAchievementId", module.RequiresAchievementId);
        command.AddParameter("@GeneratesAchievementId", module.GeneratesAchievementId);
        command.AddParameter("@BinderTypeId", module.Binder.TypeId);

        await command.Execute(connection, transaction);
    }
}