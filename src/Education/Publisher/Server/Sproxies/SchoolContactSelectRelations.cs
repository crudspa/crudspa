namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolContactSelectRelations
{
    public const String SchoolYears = "School Years";

    public static async Task<SchoolContact> Execute(String connection, SchoolContact schoolContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolContactSelectRelations";

        command.AddParameter("@SchoolContactId", schoolContact.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var relation = new RelatedEntity { Name = SchoolYears };

            while (await reader.ReadAsync())
                relation.Selections.Add(reader.ReadSelectable());

            schoolContact.Relations.Add(relation);

            return schoolContact;
        });
    }
}