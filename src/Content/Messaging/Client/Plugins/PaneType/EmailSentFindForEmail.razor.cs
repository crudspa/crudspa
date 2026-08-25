namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class EmailSentFindForEmail : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IEmailService EmailService { get; set; } = null!;

    public EmailSentFindForEmailModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var emailId = Path!.Id("email") ?? Id;

        Model = new(ScrollService, EmailService, emailId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Reset();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class EmailSentFindForEmailModel : FindModel<EmailSentSearch, EmailSent>
{
    private readonly IEmailService _emailService;
    private readonly Guid? _emailId;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public EmailSentFindForEmailModel(IScrollService scrollService, IEmailService emailService, Guid? emailId)
        : base(scrollService)
    {
        _emailService = emailService;
        _emailId = emailId;

        _sorts =
        [
            "Sent",
            "Recipient",
        ];
    }

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

        Search.ParentId = _emailId;
        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        Search.Sort.Field = Sorts.First();
        Search.Sort.Ascending = false;
        Search.ProcessedRange.Type = DateRange.Types.Any;
        Search.Statuses.Clear();

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<EmailSentSearch>(Search);
        var response = await WithWaiting("Searching...", () => _emailService.SearchSentForEmail(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }
}