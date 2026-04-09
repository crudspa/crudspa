namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class ElementType : Observable, INamed
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? IconId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? IconCssClass
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? EditorView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? RepositoryClass
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? OnlyChild
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? SupportsInteraction
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }
}