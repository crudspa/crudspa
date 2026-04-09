namespace Crudspa.Content.Design.Server.Sproxies;

public static class CourseInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Course course)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.CourseInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@TrackId", course.TrackId);
        command.AddParameter("@Title", 75, course.Title);
        command.AddParameter("@StatusId", course.StatusId);
        command.AddParameter("@Description", course.Description);
        command.AddParameter("@RequiresAchievementId", course.RequiresAchievementId);
        command.AddParameter("@GeneratesAchievementId", course.GeneratesAchievementId);
        command.AddParameter("@BinderTypeId", course.Binder.TypeId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}