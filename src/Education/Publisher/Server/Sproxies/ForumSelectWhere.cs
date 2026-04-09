using Forum = Crudspa.Education.Publisher.Shared.Contracts.Data.Forum;

namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ForumSelectWhere
{
    public static async Task<IList<Forum>> Execute(String connection, ForumSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ForumSelectWhere";

        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Districts", search.Districts);

        return await command.ReadAll(connection, ReadForum);
    }

    private static Forum ReadForum(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            Name = reader.ReadString(3),
            Description = reader.ReadString(4),
            BodyTemplateId = reader.ReadGuid(5),
            Pinned = reader.ReadBoolean(6),
            DistrictId = reader.ReadGuid(7),
            SchoolId = reader.ReadGuid(8),
            InnovatorsOnly = reader.ReadBoolean(9),
            BodyTemplateName = reader.ReadString(10),
            DistrictName = reader.ReadString(11),
            SchoolName = reader.ReadString(12),
            PostCount = reader.ReadInt32(13),
        };
    }
}