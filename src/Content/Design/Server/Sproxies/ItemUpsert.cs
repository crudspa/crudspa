namespace Crudspa.Content.Design.Server.Sproxies;

public static class ItemUpsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Item item)
    {
        if (item.Id.HasNothing())
            item.Id = Guid.NewGuid();

        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ItemUpsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", item.Id);
        command.AddParameter("@BasisId", item.BasisId);
        command.AddParameter("@BasisAmount", 10, item.BasisAmount);
        command.AddParameter("@Grow", 10, item.Grow);
        command.AddParameter("@Shrink", 10, item.Shrink);
        command.AddParameter("@AlignSelfId", item.AlignSelfId);
        command.AddParameter("@MaxWidth", 10, item.MaxWidth);
        command.AddParameter("@MinWidth", 10, item.MinWidth);
        command.AddParameter("@Width", 10, item.Width);

        await command.Execute(connection, transaction);

        return item.Id;
    }
}