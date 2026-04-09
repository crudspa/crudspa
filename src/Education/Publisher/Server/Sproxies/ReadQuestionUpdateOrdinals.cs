namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadQuestionUpdateOrdinals
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, IEnumerable<IOrderable> orderables)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadQuestionUpdateOrdinals";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Orderables", orderables);

        await command.Execute(connection, transaction);
    }
}