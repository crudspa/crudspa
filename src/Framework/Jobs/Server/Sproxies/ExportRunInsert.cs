namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class ExportRunInsert
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, ExportProcedure exportProcedure)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.ExportRunInsert";

        command.AddParameter("@ProcedureId", exportProcedure.Id);
        command.AddParameter("@Run", exportProcedure.LastRun);

        await command.Execute(connection, transaction);
    }
}