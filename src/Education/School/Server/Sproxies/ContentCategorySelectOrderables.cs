namespace Crudspa.Education.School.Server.Sproxies;

public static class ContentCategorySelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ContentCategorySelectOrderables";

        return await command.ReadOrderables(connection);
    }
}