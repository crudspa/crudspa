namespace Crudspa.Content.Design.Server.Sproxies;

public static class BinderInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Binder binder)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.BinderInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@TypeId", binder.TypeId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}