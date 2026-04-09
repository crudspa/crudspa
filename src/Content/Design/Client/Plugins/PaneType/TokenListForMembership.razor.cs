using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class TokenListForMembership : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ITokenService TokenService { get; set; } = null!;

    public TokenListForMembershipModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, TokenService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class TokenListForMembershipModel : ListOrderablesModel<TokenModel>
{
    private readonly ITokenService _tokenService;
    private readonly Guid? _membershipId;

    public TokenListForMembershipModel(IEventBus eventBus, IScrollService scrollService, ITokenService tokenService, Guid? membershipId)
        : base(scrollService)
    {
        _tokenService = tokenService;
        _membershipId = membershipId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(TokensReordered payload) => await Refresh();

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Membership>(new() { Id = _membershipId });
        var response = await WithWaiting("Fetching...", () => _tokenService.FetchForMembership(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new TokenModel(x)).ToList());
    }

    public override async Task<Response<TokenModel?>> Fetch(Guid? id)
    {
        await Task.CompletedTask;
        return new();
    }

    public override async Task<Response> Remove(Guid? id)
    {
        await Task.CompletedTask;
        return new();
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_membershipId);
    }

    public override async Task<Response> SaveOrder()
    {
        await Task.CompletedTask;
        return new();
    }
}

public class TokenModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleTokenChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Token));

    private Token _token;

    public String? Name => Token.Key;

    public TokenModel(Token token)
    {
        _token = token;
        _token.PropertyChanged += HandleTokenChanged;
    }

    public void Dispose()
    {
        _token.PropertyChanged -= HandleTokenChanged;
    }

    public Guid? Id
    {
        get => _token.Id;
        set => _token.Id = value;
    }

    public Int32? Ordinal
    {
        get => _token.Ordinal;
        set => _token.Ordinal = value;
    }

    public Token Token
    {
        get => _token;
        set => SetProperty(ref _token, value);
    }
}