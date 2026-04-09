namespace Crudspa.Framework.Core.Client.Services;

public class SessionStateCore(
    ISessionStateService sessionStateService,
    ICookieService cookieService,
    IProxyWrappers proxyWrappers)
    : ISessionState
{
    public event EventHandler? SessionInitialized;
    public event EventHandler? SessionRefreshed;
    public Session Session { get; set; } = new();
    public Boolean IsSignedIn => Session.User?.Id is not null;
    public String TimeZoneId => Session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
    public Guid? ContactId => Session.User?.Contact.Id;

    public async Task Initialize(Guid? sessionId = null, Boolean sessionPersists = true, Boolean persistSession = true)
    {
        if (sessionId is null)
        {
            var cookieValue = await cookieService.Get(Constants.CookieKeys.SessionId);
            sessionId = cookieValue.ToNullableGuid();
        }

        await Refresh(sessionId);

        await proxyWrappers.SetSessionId(Session.Id);

        if (persistSession)
        {
            if (sessionPersists)
                await cookieService.Set(Constants.CookieKeys.SessionId, $"{Session.Id:D}", DateTimeOffset.Now.AddDays(90));
            else
                await cookieService.Set(Constants.CookieKeys.SessionId, $"{Session.Id:D}", expires: null);
        }

        RaiseSessionInitialized();
    }

    public async Task Refresh(Guid? sessionId)
    {
        var request = new Request<Session>(new() { Id = sessionId });
        var response = await sessionStateService.Fetch(request);

        if (!response.Ok)
            throw new($"Session could not be fetched. {response.ErrorMessages}");

        Session = response.Value;

        RaiseSessionRefreshed();
    }

    protected virtual void RaiseSessionInitialized()
    {
        var raiseEvent = SessionInitialized;
        raiseEvent?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void RaiseSessionRefreshed()
    {
        var raiseEvent = SessionRefreshed;
        raiseEvent?.Invoke(this, EventArgs.Empty);
    }
}