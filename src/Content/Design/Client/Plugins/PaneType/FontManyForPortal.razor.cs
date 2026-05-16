namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class FontManyForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IFontFaceService FontFaceService { get; set; } = null!;
    [Inject] public IFontService FontService { get; set; } = null!;

    public FontManyForPortalModel Model { get; set; } = null!;
    public List<String> AllowedExtensions { get; set; } = [".otf", ".ttf", ".woff", ".woff2"];

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, FontService, FontFaceService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class FontManyForPortalModel : ManyModel<FontModel>,
    IHandle<FontAdded>, IHandle<FontSaved>, IHandle<FontRemoved>,
    IHandle<FontFaceAdded>, IHandle<FontFaceSaved>, IHandle<FontFaceRemoved>
{
    private readonly IFontFaceService _fontFaceService;
    private readonly IFontService _fontService;
    private readonly Guid? _contentPortalId;

    public FontManyForPortalModel(
        IEventBus eventBus,
        IScrollService scrollService,
        IFontService fontService,
        IFontFaceService fontFaceService,
        Guid? contentPortalId)
        : base(scrollService)
    {
        _fontService = fontService;
        _fontFaceService = fontFaceService;
        _contentPortalId = contentPortalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(FontAdded payload) => await Replace(payload.Id, payload.ContentPortalId);
    public async Task Handle(FontSaved payload) => await Replace(payload.Id, payload.ContentPortalId);
    public async Task Handle(FontRemoved payload) => await Rid(payload.Id, payload.ContentPortalId);
    public async Task Handle(FontFaceAdded payload) => await Replace(payload.FontId, payload.ContentPortalId);
    public async Task Handle(FontFaceSaved payload) => await Replace(payload.FontId, payload.ContentPortalId);
    public async Task Handle(FontFaceRemoved payload) => await Replace(payload.FontId, payload.ContentPortalId);

    public async Task Initialize()
    {
        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<ContentPortal>(new() { Id = _contentPortalId });
        var response = await WithWaiting("Fetching...", () => _fontService.FetchForContentPortal(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new FontModel(x)));
    }

    public override async Task Create()
    {
        var font = new Font
        {
            Id = Guid.NewGuid(),
            ContentPortalId = _contentPortalId,
            Name = "New Font",
        };

        var model = new FontModel(font, isNew: true);
        model.AddFace();

        await CreateForm(model);
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_contentPortalId);
    }

    public override async Task<Response<FontModel?>> Fetch(Guid? id)
    {
        var response = await _fontService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new FontModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<FontModel?>> Add(FormModel<FontModel> form)
    {
        var validationResponse = form.Entity.Validate();
        if (!validationResponse.Ok)
            return new() { Errors = validationResponse.Errors };

        var response = await _fontService.Add(new(form.Entity.Font));
        if (!response.Ok)
            return new() { Errors = response.Errors };

        form.Entity.Font.Id = response.Value.Id;
        var faceResponse = await form.Entity.SaveFaces(_fontFaceService);
        if (!faceResponse.Ok)
        {
            form.IsNew = false;
            return new() { Errors = faceResponse.Errors };
        }

        return new(new FontModel(form.Entity.Font));
    }

    public override async Task<Response> Save(FormModel<FontModel> form)
    {
        var font = form.Entity.Font;
        var validationResponse = form.Entity.Validate();
        if (!validationResponse.Ok)
            return validationResponse;

        var response = await _fontService.Save(new(font));

        if (!response.Ok)
            return response;

        var faceResponse = await form.Entity.SaveFaces(_fontFaceService);
        return faceResponse;
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _fontService.Remove(new(new()
        {
            Id = id,
            ContentPortalId = _contentPortalId,
        }));
    }
}

public class FontModel : Observable, IDisposable, INamed
{
    private static readonly Guid NormalStyleId = new("792f4872-173f-40bf-8426-62b6074f3267");
    private static readonly Guid ItalicStyleId = new("921b3c47-781a-4662-bd48-b224c622e048");

    private void HandleFontChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Font));
    private void HandleFaceChanged(Object? sender, PropertyChangedEventArgs args)
    {
        RaisePropertyChanged(nameof(Font));
        RaisePropertyChanged(nameof(FaceSummary));
    }

    private Font _font;
    private readonly List<FontFace> _removedFaces = [];

    public String? Name => Font.Name;
    public Boolean IsNew { get; }
    public IList<Named> StyleOptions { get; } =
    [
        new() { Id = NormalStyleId, Name = "Normal" },
        new() { Id = ItalicStyleId, Name = "Italic" },
    ];

    public FontModel(Font font, Boolean isNew = false)
    {
        IsNew = isNew;
        _font = font;
        _font.PropertyChanged += HandleFontChanged;
        _font.Faces.ForEach(SubscribeFace);
    }

    public void Dispose()
    {
        _font.PropertyChanged -= HandleFontChanged;
        _font.Faces.ForEach(UnsubscribeFace);
    }

    public Guid? Id
    {
        get => _font.Id;
        set => _font.Id = value;
    }

    public Font Font
    {
        get => _font;
        set => SetProperty(ref _font, value);
    }

    public String FaceSummary => Font.Faces.HasItems()
        ? String.Join(", ", Font.Faces.OrderBy(x => x.Style).ThenBy(x => x.WeightMin).Select(FaceLabel))
        : "No faces";

    public Guid? StyleId(FontFace face)
    {
        return String.Equals(face.Style, "italic", StringComparison.OrdinalIgnoreCase)
            ? ItalicStyleId
            : NormalStyleId;
    }

    public void SetStyle(FontFace face, Guid? id)
    {
        face.Style = id.Equals(ItalicStyleId) ? "italic" : "normal";
        RaisePropertyChanged(nameof(Font));
        RaisePropertyChanged(nameof(FaceSummary));
    }

    public void AddFace()
    {
        var weight = NextWeight();
        var face = new FontFace
        {
            FontId = Font.Id,
            Style = "normal",
            WeightMin = weight,
            WeightMax = weight,
        };

        SubscribeFace(face);
        Font.Faces.Add(face);
        RaisePropertyChanged(nameof(Font));
        RaisePropertyChanged(nameof(FaceSummary));
    }

    public void RemoveFace(FontFace face)
    {
        if (face.Id.HasValue)
            _removedFaces.Add(face);

        UnsubscribeFace(face);
        Font.Faces.Remove(face);
        RaisePropertyChanged(nameof(Font));
        RaisePropertyChanged(nameof(FaceSummary));
    }

    public async Task<Response> SaveFaces(IFontFaceService fontFaceService)
    {
        var response = new Response();

        foreach (var removedFace in _removedFaces)
        {
            var removeResponse = await fontFaceService.Remove(new(removedFace));
            if (!removeResponse.Ok)
                response.AddErrors(removeResponse.Errors);
        }

        if (!response.Ok)
            return response;

        _removedFaces.Clear();

        foreach (var face in Font.Faces)
        {
            face.FontId = Font.Id;

            if (face.Id.HasValue)
            {
                var saveResponse = await fontFaceService.Save(new(face));
                if (!saveResponse.Ok)
                    response.AddErrors(saveResponse.Errors);
            }
            else
            {
                var addResponse = await fontFaceService.Add(new(face));
                if (addResponse.Ok)
                    face.Id = addResponse.Value.Id;
                else
                    response.AddErrors(addResponse.Errors);
            }
        }

        return response;
    }

    public Response Validate()
    {
        var response = new Response();
        response.AddErrors(Font.Validate());

        foreach (var face in Font.Faces)
            response.AddErrors(face.Validate());

        return response;
    }

    public String ToFontFaceCss()
    {
        return String.Join(Environment.NewLine, Font.Faces
            .Where(x => x.FileFile.Id.HasValue)
            .Select(x =>
            {
                var weight = x.WeightMax.HasValue && x.WeightMax != x.WeightMin
                    ? $"{x.WeightMin ?? 400} {x.WeightMax}"
                    : $"{x.WeightMin ?? 400}";

                return $"@font-face{{font-family:'{Font.Name!.Replace("'", "\\'")}';src:url('{x.FileFile.FetchUrl()}');font-style:{(x.Style.HasSomething() ? x.Style : "normal")};font-weight:{weight};font-display:swap;}}";
            }));
    }

    private Int32 NextWeight()
    {
        var usedWeights = Font.Faces.Select(x => x.WeightMin ?? 400).ToHashSet();

        return new[] { 400, 700, 300, 500, 600, 800, 900, 100, 200 }
            .FirstOrDefault(x => !usedWeights.Contains(x), 400);
    }

    private static String FaceLabel(FontFace face)
    {
        var weight = face.WeightMax.HasValue && face.WeightMax != face.WeightMin
            ? $"{face.WeightMin}-{face.WeightMax}"
            : $"{face.WeightMin}";

        return $"{((face.Style ?? "normal").Equals("italic", StringComparison.OrdinalIgnoreCase) ? "Italic" : "Normal")} {weight}";
    }

    private void SubscribeFace(FontFace face)
    {
        face.PropertyChanged += HandleFaceChanged;
        face.FileFile.PropertyChanged += HandleFaceChanged;
    }

    private void UnsubscribeFace(FontFace face)
    {
        face.PropertyChanged -= HandleFaceChanged;
        face.FileFile.PropertyChanged -= HandleFaceChanged;
    }
}