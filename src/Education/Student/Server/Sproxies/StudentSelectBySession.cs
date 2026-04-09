namespace Crudspa.Education.Student.Server.Sproxies;

public static class StudentSelectBySession
{
    public static async Task<Shared.Contracts.Data.Student?> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.StudentSelectBySession";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadStudent);
    }

    private static Shared.Contracts.Data.Student ReadStudent(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ResearchId = reader.ReadString(1),
            ContactId = reader.ReadGuid(2),
            MaySignIn = reader.ReadBoolean(3),
            FirstName = reader.ReadString(4),
            LastName = reader.ReadString(5),
            FamilyId = reader.ReadGuid(6),
            StatusId = reader.ReadGuid(7),
            Email = reader.ReadString(8),
            GenderId = reader.ReadGuid(9),
            GradeId = reader.ReadGuid(10),
            AssessmentTypeGroupId = reader.ReadGuid(11),
            AssessmentLevelGroupId = reader.ReadGuid(12),
            ConditionGroupId = reader.ReadGuid(13),
            GoalSettingGroupId = reader.ReadGuid(14),
            PersonalizationGroupId = reader.ReadGuid(15),
            ContentGroupId = reader.ReadGuid(16),
            AudioGenderId = reader.ReadGuid(17),
            ChallengeLevelId = reader.ReadGuid(18),
            PreferredName = reader.ReadString(19),
            AvatarString = reader.ReadString(20),
            ResearchGroupId = reader.ReadGuid(21),
            AudioGenderName = reader.ReadString(22),
            ChallengeLevelName = reader.ReadString(23),
            FamilySchoolId = reader.ReadGuid(24),
            FamilySchoolKey = reader.ReadString(25),
            FamilySchoolOrganizationName = reader.ReadString(26),
            FamilySchoolDistrictOrganizationName = reader.ReadString(27),
            GradeName = reader.ReadString(28),
            StatusName = reader.ReadString(29),
            TimeZoneId = reader.ReadString(30),
            StudentBookCount = reader.ReadInt32(31),
        };
    }
}