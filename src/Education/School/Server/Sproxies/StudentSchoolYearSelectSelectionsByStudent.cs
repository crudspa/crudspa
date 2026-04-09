namespace Crudspa.Education.School.Server.Sproxies;

public static class StudentSchoolYearSelectSelectionsByStudent
{
    public static async Task<IList<Selectable>> Execute(String connection, Guid? studentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.StudentSchoolYearSelectSelectionsByStudent";

        command.AddParameter("@StudentId", studentId);

        return await command.ReadSelectables(connection);
    }
}