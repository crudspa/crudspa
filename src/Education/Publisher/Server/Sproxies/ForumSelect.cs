using Forum = Crudspa.Education.Publisher.Shared.Contracts.Data.Forum;

namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ForumSelect
{
    public static async Task<Forum?> Execute(String connection, Forum forum)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ForumSelect";

        command.AddParameter("@Id", forum.Id);

        return await command.ReadSingle(connection, ReadForum);
    }

    private static Forum ReadForum(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            Description = reader.ReadString(2),
            BodyTemplateId = reader.ReadGuid(3),
            Pinned = reader.ReadBoolean(4),
            DistrictId = reader.ReadGuid(5),
            SchoolId = reader.ReadGuid(6),
            InnovatorsOnly = reader.ReadBoolean(7),
            BodyTemplateName = reader.ReadString(8),
            DistrictStudentIdNumberLabel = reader.ReadString(9),
            SchoolKey = reader.ReadString(10),
            PostCount = reader.ReadInt32(11),
        };
    }
}