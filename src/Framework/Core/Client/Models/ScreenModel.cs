using System.Collections.Specialized;

namespace Crudspa.Framework.Core.Client.Models;

public class ScreenModel : Observable, IDisposable
{
    private void HandleAlertsChanged(Object? sender, NotifyCollectionChangedEventArgs args) => RaisePropertyChanged(nameof(Alerts));

    private const String DefaultWaitingOn = "Loading...";

    private String _waitingOn;

    public ObservableCollection<Alert> Alerts { get; } = [];

    public ScreenModel()
    {
        _waitingOn = DefaultWaitingOn;
        Alerts.CollectionChanged += HandleAlertsChanged;
    }

    public virtual void Dispose()
    {
        Alerts.CollectionChanged -= HandleAlertsChanged;
    }

    public Boolean Waiting
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String WaitingOn
    {
        get => _waitingOn;
        set => SetProperty(ref _waitingOn, value);
    }

    public async Task<Response<T>> WithAlerts<T>(Func<Task<Response<T>>> func, Boolean dismissExisting = true) where T : class?
    {
        if (dismissExisting)
            Alerts.RemoveWhere(x => x.Dismissible);

        var response = await func.Invoke();

        if (response.Errors.HasItems())
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Error,
                Errors = response.Errors,
            });
        else if (!response.Ok)
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Error,
                Errors = new List<Error> { new() { Message = "Request failed. (No error details were provided.)" } },
            });

        return response;
    }

    public async Task<Response> WithAlerts(Func<Task<Response>> func, Boolean dismissExisting = true)
    {
        if (dismissExisting)
            Alerts.RemoveWhere(x => x.Dismissible);

        var response = await func.Invoke();

        if (response.Errors.HasItems())
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Error,
                Errors = response.Errors,
            });
        else if (!response.Ok)
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Error,
                Errors = new List<Error> { new() { Message = "Request failed. (No error details were provided.)" } },
            });

        return response;
    }

    public async Task<Response<T>> WithWaiting<T>(Func<Task<Response<T>>> func) where T : class
    {
        WaitingOn = DefaultWaitingOn;
        Waiting = true;

        var response = await WithAlerts(func);

        Waiting = false;

        return response;
    }

    public async Task<Response<T>> WithWaiting<T>(String waitingOn, Func<Task<Response<T>>> func, Boolean resetAlerts = true) where T : class?
    {
        WaitingOn = waitingOn;
        Waiting = true;

        var response = await WithAlerts(func, resetAlerts);

        Waiting = false;

        return response;
    }

    public async Task<Response> WithWaiting(Func<Task<Response>> func)
    {
        WaitingOn = DefaultWaitingOn;
        Waiting = true;

        var response = await WithAlerts(func);

        Waiting = false;

        return response;
    }

    public async Task<Response> WithWaiting(String waitingOn, Func<Task<Response>> func)
    {
        WaitingOn = waitingOn;
        Waiting = true;

        var response = await WithAlerts(func);

        Waiting = false;

        return response;
    }

    public async Task WithMany(String waitingOn, params Task[] tasks)
    {
        WaitingOn = waitingOn;
        Waiting = true;

        await Task.WhenAll(tasks);

        Waiting = false;
    }

    public Boolean IsValid(IValidates entity)
    {
        Alerts.RemoveWhere(x => x.Dismissible);

        var errors = entity.Validate();

        if (errors.HasItems())
        {
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Error,
                Errors = errors,
            });

            return false;
        }

        return true;
    }
}