namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Forum : Observable, IValidates, IOrderable
{
    public enum AccessModes
    {
        Everyone,
        LicensedUsers,
    }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PortalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PortalKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PermissionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PermissionName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AccessModes AccessMode
    {
        get;
        set => SetProperty(ref field, value);
    } = AccessModes.LicensedUsers;

    public String? Description
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ObservableCollection<Selectable> Licenses
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<ForumBundle> ForumBundles
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (!Enum.IsDefined(AccessMode))
                errors.AddError("Access mode is invalid.", nameof(AccessMode));

            var selectedLicenses = Licenses.Where(x => x.Selected == true).ToList();

            if (AccessMode == AccessModes.LicensedUsers && selectedLicenses.Count == 0)
                errors.AddError("At least one license is required for licensed access.", nameof(Licenses));

            if (selectedLicenses.Any(x => !x.Id.HasValue))
                errors.AddError("A selected license is invalid.", nameof(Licenses));

            if (selectedLicenses.Where(x => x.Id.HasValue).GroupBy(x => x.Id).Any(x => x.Count() > 1))
                errors.AddError("A license cannot be selected more than once.", nameof(Licenses));

            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 150)
                errors.AddError("Title cannot be longer than 150 characters.", nameof(Title));

            if (Description.HasNothing())
                errors.AddError("Description is required.", nameof(Description));
            else if (Description!.Length > ForumPolicy.MaxForumDescriptionCharacters)
                errors.AddError($"Description cannot be longer than {ForumPolicy.MaxForumDescriptionCharacters:N0} characters.", nameof(Description));

            foreach (var forumBundle in ForumBundles)
                errors.AddRange(forumBundle.Validate());

            if (ForumBundles.Where(x => x.BundleId.HasValue).GroupBy(x => x.BundleId).Any(x => x.Count() > 1))
                errors.AddError("A tag bundle cannot be configured more than once.", nameof(ForumBundles));
        });
    }
}