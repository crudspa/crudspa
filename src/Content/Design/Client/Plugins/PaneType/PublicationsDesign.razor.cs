using Crudspa.Content.Display.Shared.Contracts.Config.PaneType;

namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class PublicationsDesign : IPaneDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public String? Path { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public EventCallback ConfigUpdated { get; set; }
    [Inject] public IPublicationGroupService GroupService { get; set; } = null!;
    [Inject] public IPublicationService PublicationService { get; set; } = null!;

    public PublicationsConfig Config { get; private set; } = new();
    public IList<PublicationGroup> Groups { get; private set; } = [];
    public IList<Crudspa.Content.Display.Shared.Contracts.Data.PublicationSummary>? Preview { get; private set; }
    public String? PreviewError { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        Config = ConfigJson.FromJson<PublicationsConfig>() ?? new();
        var response = await GroupService.FetchForPortal(new(new() { Id = Path!.Id("portal") }));
        if (response.Ok) Groups = response.Value;
    }

    public Task<Boolean> PrepareForSave() => Task.FromResult(!Config.GroupId.HasValue || Groups.Any(x => x.Id == Config.GroupId));
    public String? GetConfigJson() => Config.ToJson();

    private async Task LoadPreview()
    {
        var response = await PublicationService.FetchForPortal(new(new() { Id = Path!.Id("portal") }));
        PreviewError = response.Ok ? null : response.ErrorMessages;
        if (!response.Ok) return;
        // Draft preview uses the privileged authoring path; the anonymous endpoint never accepts a draft flag.
        Preview = response.Value.Select(x => new Crudspa.Content.Display.Shared.Contracts.Data.PublicationSummary
        {
            Id = x.Id, Title = x.Title, Citation = x.Citation, Venue = x.Venue, Year = x.Year,
            GroupId = x.GroupId, GroupName = x.GroupName, GroupOrdinal = Groups.FirstOrDefault(g => g.Id == x.GroupId)?.Ordinal,
            StatusId = x.StatusId, StatusName = x.StatusName, Ordinal = x.Ordinal,
            Sample = x.Sample, Timeline = x.Timeline, Outcome = x.Outcome, Intervention = x.Intervention,
            ImageFile = x.ImageFile, ImageAlt = x.ImageAlt,
            PublicationResources = new(x.PublicationResources.Select(r => new Crudspa.Content.Display.Shared.Contracts.Data.PublicationResourceSummary
            {
                Id = r.Id, PublicationId = r.PublicationId, TypeId = r.TypeId, TypeName = r.TypeName,
                Label = r.Label, Url = r.Url, PdfFile = r.PdfFile, ImageFile = r.ImageFile, Ordinal = r.Ordinal,
            }))
        }).ToList();
    }
}