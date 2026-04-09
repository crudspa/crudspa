namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class UnitBookSummary : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UnitId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Treatment
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? Control
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public Book? Book
    {
        get;
        set => SetProperty(ref field, value);
    }
}