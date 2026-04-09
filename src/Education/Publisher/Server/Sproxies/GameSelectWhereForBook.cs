namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameSelectWhereForBook
{
    public static async Task<IList<Game>> Execute(String connection, Guid? sessionId, GameSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameSelectWhereForBook";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BookId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Status", search.Status);
        command.AddParameter("@Grades", search.Grades);
        command.AddParameter("@AssessmentLevels", search.AssessmentLevels);

        return await command.ReadAll(connection, ReadGame);
    }

    private static Game ReadGame(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            BookId = reader.ReadGuid(3),
            BookKey = reader.ReadString(4),
            Key = reader.ReadString(5),
            Title = reader.ReadString(6),
            StatusId = reader.ReadGuid(7),
            StatusName = reader.ReadString(8),
            IconId = reader.ReadGuid(9),
            IconName = reader.ReadString(10),
            GradeId = reader.ReadGuid(11),
            GradeName = reader.ReadString(12),
            AssessmentLevelId = reader.ReadGuid(13),
            AssessmentLevelKey = reader.ReadString(14),
            RequiresAchievementId = reader.ReadGuid(15),
            RequiresAchievementTitle = reader.ReadString(16),
            GeneratesAchievementId = reader.ReadGuid(17),
            GeneratesAchievementTitle = reader.ReadString(18),
            GameSectionCount = reader.ReadInt32(19),
        };
    }
}