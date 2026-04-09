using Achievement = Crudspa.Education.Publisher.Shared.Contracts.Data.Achievement;

namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AchievementSelectWhere
{
    public static async Task<IList<Achievement>> Execute(String connection, Guid? sessionId, AchievementSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AchievementSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Rarities", search.Rarities);

        return await command.ReadAll(connection, ReadAchievement);
    }

    private static Achievement ReadAchievement(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            Title = reader.ReadString(3),
            RarityId = reader.ReadGuid(4),
            RarityName = reader.ReadString(5),
            TrophyImageFile = new()
            {
                Id = reader.ReadGuid(6),
                BlobId = reader.ReadGuid(7),
                Name = reader.ReadString(8),
                Format = reader.ReadString(9),
                Width = reader.ReadInt32(10),
                Height = reader.ReadInt32(11),
                Caption = reader.ReadString(12),
            },
            VisibleToStudents = reader.ReadBoolean(13),
        };
    }
}