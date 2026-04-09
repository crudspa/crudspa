namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Style : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContentPortalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? RuleId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ConfigJson
    {
        get;
        set => SetProperty(ref field, value);
    }

    public RuleFull Rule
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!RuleId.HasValue)
                errors.AddError("Rule is required.", nameof(RuleId));

            if (ConfigJson.HasNothing())
                errors.AddError("Config Json is required.", nameof(ConfigJson));
        });
    }
}