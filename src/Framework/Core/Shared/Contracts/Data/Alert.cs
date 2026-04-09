namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Alert : Observable
{
    public enum AlertType
    {
        Error,
        Lock,
        Success,
        Tip,
        Warning,
    }

    public AlertType Type
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Message
    {
        get;
        set => SetProperty(ref field, value);
    }

    public IList<Error> Errors
    {
        get;
        set => SetProperty(ref field, value);
    } = (List<Error>)[];

    public Boolean Dismissible
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public String Icon
    {
        get
        {
            switch (Type)
            {
                case AlertType.Error:
                    return "c-icon-cross-circle";
                case AlertType.Lock:
                    return "c-icon-lock";
                case AlertType.Success:
                    return "c-icon-checkmark-circle";
                case AlertType.Tip:
                    return "c-icon-bulb";
                case AlertType.Warning:
                    return "c-icon-warning";
                default:
                    return String.Empty;
            }
        }
    }
}