using Crudspa.Education.Common.Client.Plugins;

namespace Crudspa.Education.Common.Client.Models;

public class GameActivityModel : Observable, IDisposable, INamed, IOrderable
{
    public event EventHandler<Guid>? ActivityCompleted;

    private GameActivity _gameActivity;

    public String Name => GameActivity.Activity!.ActivityTypeCategoryName + " - " + GameActivity.Activity.ActivityTypeName + " - " + GameActivity.Activity.ContentAreaName;

    public GameActivityModel(GameActivity gameActivity)
    {
        _gameActivity = gameActivity;
        _gameActivity.PropertyChanged += HandleGameActivityChanged;
    }

    public void Dispose()
    {
        _gameActivity.PropertyChanged -= HandleGameActivityChanged;
    }

    public void HandleActivityCompleted(Guid status)
    {
        RaiseActivityCompleted(status);
    }

    private void HandleGameActivityChanged(Object? sender, PropertyChangedEventArgs args)
    {
        RaisePropertyChanged(nameof(GameActivity));
    }

    public Guid? Id
    {
        get => _gameActivity.Id;
        set => _gameActivity.Id = value;
    }

    public Int32? Ordinal
    {
        get => _gameActivity.Ordinal;
        set => _gameActivity.Ordinal = value;
    }

    public GameActivity GameActivity
    {
        get => _gameActivity;
        set => SetProperty(ref _gameActivity, value);
    }

    public ActivityDisplayPlugin ActivityDisplay { get; set; } = null!;

    private void RaiseActivityCompleted(Guid status)
    {
        var raiseEvent = ActivityCompleted;
        raiseEvent?.Invoke(this, status);
    }
}