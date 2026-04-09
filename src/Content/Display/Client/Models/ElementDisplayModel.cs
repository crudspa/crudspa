namespace Crudspa.Content.Display.Client.Models;

public class ElementDisplayModel : Observable, IHandle<ElementProgressUpdated>
{
    public enum CompletionStatuses { Error, Incomplete, Complete }

    private readonly IEventBus _eventBus;
    private readonly IElementProgressService _elementProgressService;

    private ElementProgress? _progress;

    public SectionElement SectionElement { get; }
    public Element Element => SectionElement.Element;

    public ElementDisplayModel(IEventBus eventBus,
        IElementProgressService elementProgressService,
        SectionElement element)
    {
        _eventBus = eventBus;
        _elementProgressService = elementProgressService;

        SectionElement = element;

        _eventBus.Subscribe(this);
    }

    public Task Handle(ElementProgressUpdated payload)
    {
        if (!payload.Progress.ElementId.Equals(Element.Id))
            return Task.CompletedTask;

        _progress = payload.Progress;
        NeedsAttention = _progress.TimesCompleted == 0;
        RaisePropertyChanged(nameof(CompletionStatus));

        return Task.CompletedTask;
    }

    public Boolean NeedsAttention
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Replaying
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task InitializeProgress()
    {
        _progress = await _elementProgressService.Fetch(new(new() { Id = Element.Id }));
        RaisePropertyChanged(nameof(CompletionStatus));
    }

    public CompletionStatuses CompletionStatus
    {
        get
        {
            if (NeedsAttention)
                return CompletionStatuses.Error;

            if (_progress?.TimesCompleted is not null && _progress.TimesCompleted > 0)
                return CompletionStatuses.Complete;

            return CompletionStatuses.Incomplete;
        }
    }

    public async Task AddElementCompleted()
    {
        Replaying = false;
        NeedsAttention = false;

        var request = new Request<ElementCompleted>(new()
        {
            ElementId = Element.Id,
            DeviceTimestamp = DateTimeOffset.Now,
        });

        await _elementProgressService.AddCompleted(request);

        await _eventBus.Publish(new ValidateBinder());
    }

    public void MarkElementIncorrect()
    {
        Replaying = false;
        NeedsAttention = true;

        if (_progress?.TimesCompleted == 0)
            _progress.TimesCompleted++;
    }

    public T RequireConfig<T>() where T : class => SectionElement.RequireConfig<T>();
}