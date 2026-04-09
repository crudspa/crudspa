namespace Crudspa.Framework.Core.Shared.BaseClasses;

public abstract class Observable : IObservable
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public Boolean SetProperty<T>(ref T field, T value, [CallerMemberName] String? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;

        field = value;
        RaisePropertyChanged(propertyName);
        return true;
    }

    public void RaisePropertyChanged([CallerMemberName] String? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}