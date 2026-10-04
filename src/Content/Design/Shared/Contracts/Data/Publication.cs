
namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class Publication : Observable, IValidates, INamed, IOrderable
{
    public String? Name => Title;

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

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GroupId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GroupName
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

    public String? Citation
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Venue
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Sample
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Timeline
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Outcome
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Intervention
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? ImageAlt
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Year
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<PublicationResource> PublicationResources
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!String.IsNullOrWhiteSpace(ImageFile.Name) && !ImageFile.Id.HasValue && !ImageFile.BlobId.HasValue)
                errors.AddError("Wait for the cover upload to finish before saving.");
            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 300)
                errors.AddError("Title cannot be longer than 300 characters.", nameof(Title));

            if (StatusId == Crudspa.Framework.Core.Shared.Contracts.Ids.ContentStatusIds.Complete && String.IsNullOrWhiteSpace(Citation))
                errors.AddError("Citation is required before publication.", nameof(Citation));
            if (Year is < 1000 or > 9999)
                errors.AddError("Enter a four-digit publication year.", nameof(Year));
            foreach (var value in new[] { (Venue, 250), (Sample, 500), (Timeline, 100), (Outcome, 1000), (Intervention, 1000), (ImageAlt, 300) })
                if (value.Item1?.Length > value.Item2)
                    errors.AddError($"A publication field exceeds its {value.Item2}-character limit.");

            PublicationResources.Apply(x => errors.AddRange(x.Validate()));
        });
    }
}