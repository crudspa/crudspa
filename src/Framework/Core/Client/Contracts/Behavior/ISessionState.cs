namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface ISessionState
{
    event EventHandler SessionInitialized;
    event EventHandler SessionRefreshed;
    Session Session { get; set; }
    Task Initialize(Guid? sessionId = null, Boolean sessionsPersist = true, Boolean persistSession = true);
    Boolean IsSignedIn { get; }
    String TimeZoneId { get; }
    Guid? ContactId { get; }
    Task Refresh(Guid? sessionId);
}