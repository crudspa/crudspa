namespace Crudspa.Content.Design.Server.Sproxies;

public static class ContainerUpsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Container container)
    {
        if (container.Id.HasNothing())
            container.Id = Guid.NewGuid();

        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ContainerUpsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", container.Id);
        command.AddParameter("@DirectionId", container.DirectionId);
        command.AddParameter("@WrapId", container.WrapId);
        command.AddParameter("@JustifyContentId", container.JustifyContentId);
        command.AddParameter("@AlignItemsId", container.AlignItemsId);
        command.AddParameter("@AlignContentId", container.AlignContentId);
        command.AddParameter("@Gap", 10, container.Gap);

        await command.Execute(connection, transaction);

        return container.Id;
    }
}