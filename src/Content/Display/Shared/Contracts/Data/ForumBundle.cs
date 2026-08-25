namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class ForumBundle : Observable, IValidates
{
    public enum Rules
    {
        NotUsed,
        Optional,
        Required,
    }

    public Guid? ForumId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BundleId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BundleName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Rules ThreadRule
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Rules CommentRule
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Selectable> Tags
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!BundleId.HasValue)
                errors.AddError("Tag bundle is required.", nameof(BundleId));

            if (!Enum.IsDefined(ThreadRule))
                errors.AddError("Thread rule is invalid.", nameof(ThreadRule));

            if (!Enum.IsDefined(CommentRule))
                errors.AddError("Comment rule is invalid.", nameof(CommentRule));
        });
    }

    public static List<Error> ValidateSelection(IEnumerable<ForumBundle> bundles,
        IEnumerable<Selectable> selectedTags, Boolean forComments, String fieldName)
    {
        var errors = new List<Error>();
        var configured = bundles.ToList();
        var selected = selectedTags.Where(x => x.Selected == true).ToList();
        var selectedIds = selected.Where(x => x.Id.HasValue).Select(x => x.Id).ToHashSet();

        if (selected.Any(x => !x.Id.HasValue))
            errors.AddError("A selected tag is invalid.", fieldName);

        if (selected.Where(x => x.Id.HasValue).GroupBy(x => x.Id).Any(x => x.Count() > 1))
            errors.AddError("A tag cannot be selected more than once.", fieldName);

        var enabledTagIds = configured
            .Where(x => (forComments ? x.CommentRule : x.ThreadRule) != Rules.NotUsed)
            .SelectMany(x => x.Tags)
            .Where(x => x.Id.HasValue)
            .Select(x => x.Id)
            .ToHashSet();

        if (selectedIds.Any(x => !enabledTagIds.Contains(x)))
            errors.AddError("A selected tag is not available for this forum.", fieldName);

        foreach (var bundle in configured)
        {
            var rule = forComments ? bundle.CommentRule : bundle.ThreadRule;
            var bundleTagIds = bundle.Tags.Where(x => x.Id.HasValue).Select(x => x.Id).ToHashSet();
            var hasSelection = selectedIds.Any(bundleTagIds.Contains);

            if (rule == Rules.Required && !hasSelection)
                errors.AddError($"Select at least one tag from {bundle.BundleName ?? "the required bundle"}.", fieldName);
        }

        return errors;
    }
}