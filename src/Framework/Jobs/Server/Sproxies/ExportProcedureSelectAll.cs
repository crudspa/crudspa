namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class ExportProcedureSelectAll
{
    public static async Task<IList<ExportProcedure>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.ExportProcedureSelectAll";

        return await command.ReadAll(connection, ReadExportProcedure);
    }

    private static ExportProcedure ReadExportProcedure(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ProcedureName = reader.ReadString(1),
            LastRun = reader.ReadDateTimeOffset(2),
        };
    }
}