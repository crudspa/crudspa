namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Classroom classroom)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Name", 75, classroom.OrganizationName);
        command.AddParameter("@TypeId", classroom.TypeId);
        command.AddParameter("@GradeId", classroom.GradeId);
        command.AddParameter("@SmallClassroom", classroom.SmallClassroom ?? false);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}