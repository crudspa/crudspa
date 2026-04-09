namespace Crudspa.Education.District.Server.Sproxies;

public static class TitleSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.TitleSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}