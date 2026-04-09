namespace Crudspa.Education.Common.Server.Sproxies;

public static class GradeSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.GradeSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}