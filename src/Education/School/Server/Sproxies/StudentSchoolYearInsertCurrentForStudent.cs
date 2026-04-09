namespace Crudspa.Education.School.Server.Sproxies;

public static class StudentSchoolYearInsertCurrentForStudent
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Student student)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.StudentSchoolYearInsertCurrentForStudent";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@StudentId", student.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}