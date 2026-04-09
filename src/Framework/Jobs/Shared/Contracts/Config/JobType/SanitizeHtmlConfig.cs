namespace Crudspa.Framework.Jobs.Shared.Contracts.Config.JobType;

public class SanitizeHtmlConfig : Observable
{
    public Int32? LimitRowsPerTarget
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (LimitRowsPerTarget is < 1)
                errors.AddError("Row limit must be at least 1 when supplied.", nameof(LimitRowsPerTarget));
        });
    }
}