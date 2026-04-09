namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Classroom classroom)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", classroom.Id);
        command.AddParameter("@Name", 75, classroom.OrganizationName);
        command.AddParameter("@TypeId", classroom.TypeId);
        command.AddParameter("@GradeId", classroom.GradeId);
        command.AddParameter("@SmallClassroom", classroom.SmallClassroom ?? false);

        await command.Execute(connection, transaction);
    }
}