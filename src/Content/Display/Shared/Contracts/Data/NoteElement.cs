namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class NoteElement : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ElementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Instructions
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile ImageFileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Boolean? RequireText
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequireImageSelection
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public ObservableCollection<NoteImage> NoteImages
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Instructions.HasNothing())
                errors.AddError("Instructions are required.", nameof(Instructions));

            if (!RequireText.HasValue)
                errors.AddError("Require Text is required.", nameof(RequireText));

            if (!RequireImageSelection.HasValue)
                errors.AddError("Require Image Selection is required.", nameof(RequireImageSelection));
        });
    }
}