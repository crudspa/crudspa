namespace Crudspa.Education.School.Server.Sproxies;

public static class StudentSchoolYearUpdateSelectionsByStudent
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Student student)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.StudentSchoolYearUpdateSelectionsByStudent";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@StudentId", student.Id);
        command.AddParameter("@SchoolYears", student.SchoolYears);

        await command.Execute(connection, transaction);
    }
}