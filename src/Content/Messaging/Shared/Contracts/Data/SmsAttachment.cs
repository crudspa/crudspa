namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class SmsAttachment : Observable, IValidates, IOrderable
{

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SmsId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SmsBody
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }


    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (ImageFile.Name.HasNothing())
                errors.AddError("Image is required.", nameof(ImageFile));
        });
    }
}