namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class ContentPortal : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? MaxWidth
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? StyleRevision
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? StyleCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? FontCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile BrandingImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? SeoTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SeoDescription
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SeoKeywords
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile SeoImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? CanonicalBaseUrl
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Portal Portal
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Guid? FooterPageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? AchievementCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? BlogCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ForumCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TrackCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Page? FooterPage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            errors.AddRange(Portal.Validate());

            if (SeoTitle.HasSomething() && SeoTitle!.Length > 100)
                errors.AddError("SEO Title cannot be longer than 100 characters.", nameof(SeoTitle));

            if (SeoDescription.HasSomething() && SeoDescription!.Length > 300)
                errors.AddError("SEO Description cannot be longer than 300 characters.", nameof(SeoDescription));

            if (SeoKeywords.HasSomething() && SeoKeywords!.Length > 300)
                errors.AddError("SEO Keywords cannot be longer than 300 characters.", nameof(SeoKeywords));

            if (CanonicalBaseUrl.HasSomething())
            {
                if (CanonicalBaseUrl!.Length > 250)
                    errors.AddError("Canonical Base URL cannot be longer than 250 characters.", nameof(CanonicalBaseUrl));
                else if (!Uri.TryCreate(CanonicalBaseUrl, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                    errors.AddError("Canonical Base URL must be an absolute HTTP or HTTPS URL.", nameof(CanonicalBaseUrl));
            }
        });
    }
}