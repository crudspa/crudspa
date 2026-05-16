namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class FontFace : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? FontId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public FontFile FileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? Style
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? WeightMin
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? WeightMax
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (FileFile.Name.HasNothing() || (!FileFile.BlobId.HasValue && !FileFile.Id.HasValue))
                errors.AddError("File is required.", nameof(FileFile));

            if (Style.HasNothing())
                errors.AddError("Style is required.", nameof(Style));
            else if (Style!.Length > 10)
                errors.AddError("Style cannot be longer than 10 characters.", nameof(Style));
            else if (Style != "normal" && Style != "italic")
                errors.AddError("Style must be normal or italic.", nameof(Style));

            if (!WeightMin.HasValue)
                errors.AddError("Weight Min is required.", nameof(WeightMin));
            else if (WeightMin < 1 || WeightMin > 1000)
                errors.AddError("Weight Min must be between 1 and 1000.", nameof(WeightMin));

            if (!WeightMax.HasValue)
                errors.AddError("Weight Max is required.", nameof(WeightMax));
            else if (WeightMax < 1 || WeightMax > 1000)
                errors.AddError("Weight Max must be between 1 and 1000.", nameof(WeightMax));

            if (WeightMin.HasValue && WeightMax.HasValue && WeightMax < WeightMin)
                errors.AddError("Weight Max must be greater than or equal to Weight Min.", nameof(WeightMax));
        });
    }
}