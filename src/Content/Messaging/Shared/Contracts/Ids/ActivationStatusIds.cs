namespace Crudspa.Content.Messaging.Shared.Contracts.Ids;

public static class ActivationStatusIds
{
    public static Guid Draft => Guid.Parse("a9b01001-7c15-4fb8-b25f-9f5629031001");
    public static Guid Scheduled => Guid.Parse("a9b01002-7c15-4fb8-b25f-9f5629031002");
    public static Guid Active => Guid.Parse("a9b01003-7c15-4fb8-b25f-9f5629031003");
    public static Guid Completed => Guid.Parse("a9b01004-7c15-4fb8-b25f-9f5629031004");
    public static Guid NeedsAttention => Guid.Parse("a9b01005-7c15-4fb8-b25f-9f5629031005");
}