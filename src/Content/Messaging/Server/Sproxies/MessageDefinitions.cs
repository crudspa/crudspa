namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class MessageDefinitions
{
    public static Message Read(SqlDataReader reader) => new() { Id=reader.ReadGuid(0),StageId=reader.ReadGuid(1),Name=reader.ReadString(2),PopulationId=reader.ReadGuid(3),MessageTypeId=reader.ReadGuid(4),EmailTemplateId=reader.ReadGuid(5),SmsTemplateId=reader.ReadGuid(6),Ordinal=reader.ReadInt32(7) };
}

public static class MessageSelectForStage
{
    public static async Task<IList<Message>> Execute(String connection,Guid? sessionId,Guid? stageId)
    {
        await using var command=new SqlCommand { CommandText="ContentMessaging.MessageSelectForStage" };
        command.AddParameter("@SessionId",sessionId); command.AddParameter("@StageId",stageId);
        return await command.ExecuteQuery(connection,async reader=>{var values=new List<Message>();while(await reader.ReadAsync())values.Add(MessageDefinitions.Read(reader));return values;});
    }
}

public static class MessageSelect
{
    public static async Task<Message?> Execute(String connection,Guid? sessionId,Guid? id)
    {
        await using var command=new SqlCommand { CommandText="ContentMessaging.MessageSelect" };
        command.AddParameter("@SessionId",sessionId); command.AddParameter("@Id",id); return await command.ReadSingle(connection,MessageDefinitions.Read);
    }
}

public static class MessageInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection,SqlTransaction? transaction,Guid? sessionId,Message message)
    {
        await using var command=Command("ContentMessaging.MessageInsert",sessionId,message);var output=command.AddOutputParameter("@Id");await command.Execute(connection,transaction);return (Guid?)output.Value;
    }
    internal static SqlCommand Command(String text,Guid? sessionId,Message message){var command=new SqlCommand{CommandText=text};command.AddParameter("@SessionId",sessionId);command.AddParameter("@StageId",message.StageId);command.AddParameter("@Name",message.Name);command.AddParameter("@PopulationId",message.PopulationId);command.AddParameter("@MessageTypeId",message.MessageTypeId);command.AddParameter("@EmailTemplateId",message.EmailTemplateId);command.AddParameter("@SmsTemplateId",message.SmsTemplateId);return command;}
}

public static class MessageUpdate
{
    public static async Task Execute(SqlConnection connection,SqlTransaction? transaction,Guid? sessionId,Message message){await using var command=MessageInsert.Command("ContentMessaging.MessageUpdate",sessionId,message);command.AddParameter("@Id",message.Id);await command.Execute(connection,transaction);}
}

public static class MessageDelete
{
    public static async Task Execute(SqlConnection connection,SqlTransaction? transaction,Guid? sessionId,Guid? id){await using var command=new SqlCommand{CommandText="ContentMessaging.MessageDelete"};command.AddParameter("@SessionId",sessionId);command.AddParameter("@Id",id);await command.Execute(connection,transaction);}
}