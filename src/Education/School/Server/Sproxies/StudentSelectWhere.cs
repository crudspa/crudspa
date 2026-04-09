using Crudspa.Education.School.Shared.Extensions;

namespace Crudspa.Education.School.Server.Sproxies;

public static class StudentSelectWhere
{
    public static async Task<IList<Student>> Execute(String connection, Guid? sessionId, StudentSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.StudentSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Grades", search.Grades);
        command.AddParameter("@AssessmentLevelGroups", search.AssessmentLevelGroups);
        command.AddParameter("@SchoolYears", search.SchoolYears);
        command.AddParameter("@NotInClassroom", search.NotInClassroom);
        command.AddParameter("@IncludeTestAccounts", search.IncludeTestAccounts);

        return await command.ReadAll(connection, ReadStudent);
    }

    private static Student ReadStudent(SqlDataReader reader)
    {
        var student = new Student
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            FirstName = reader.ReadString(3),
            LastName = reader.ReadString(4),
            SecretCode = reader.ReadString(5),
            GradeId = reader.ReadGuid(6),
            AssessmentLevelGroupId = reader.ReadGuid(7),
            PreferredName = reader.ReadString(8),
            AvatarString = reader.ReadString(9),
            IdNumber = reader.ReadString(10),
            IsTestAccount = reader.ReadBoolean(11),
            FamilySchoolId = reader.ReadGuid(12),
            SchoolName = reader.ReadString(13),
            GradeName = reader.ReadString(14),
        };

        student.FriendlyAssessmentLevel = student.AssessmentLevelGroupId.ToFriendlyName();

        return student;
    }
}