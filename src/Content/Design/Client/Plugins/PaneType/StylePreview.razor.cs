using Crudspa.Framework.Core.Shared.Contracts.Ids;

namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class StylePreview : IPaneDisplay, IDisposable,
    IHandle<StyleSaved>, IHandle<FontSaved>, IHandle<FontRemoved>
{
    private sealed record PreviewAlert(String TypeCss, String IconCss, String Message);
    private sealed record PreviewContentStatus(Guid Id, String Name);

    private static readonly Guid AudienceFamiliesId = new("1cfc5c29-b125-46d9-8953-f0d1e35897ef");
    private static readonly Guid AudienceEducatorsId = new("c8ecdc18-c8e0-4763-83de-d7928185c40c");
    private static readonly Guid AudiencePartnersId = new("4f1c598f-3e7d-4775-ac23-b1eaf594efeb");
    private static readonly Guid ChannelWebsiteId = new("7d31c6ad-caea-4a92-81d0-b412bfe8850b");
    private static readonly Guid ChannelEmailId = new("387c6620-4ce2-4a44-b78f-4fd8c5c7e07e");
    private static readonly Guid ChannelDashboardId = new("775508d1-04d8-4ae7-a6ce-ae77b237d703");
    private static readonly Guid ContentTypeArticleId = new("60afec86-8b63-47fe-8d8d-57a5ef5d9d11");
    private static readonly Guid ContentTypeCollectionId = new("49e54b74-2d86-44bd-8fda-fbfa5a9b80cb");
    private static readonly Guid ContentTypePromoId = new("4ed3f341-a9f1-4f2d-a5d3-25efbbd068a9");
    private static readonly Guid WorkflowDraftId = new("e183b64c-8b15-44f6-9f45-18d2d365f4c7");
    private static readonly Guid WorkflowReviewId = new("1f4fa7ff-b6a3-4417-aac8-276c5235c555");
    private static readonly Guid WorkflowPublishedId = new("ea1f7698-5685-4ff6-8af6-f0bf5295548a");

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;

    public Int32 PreviewVersion { get; set; }
    public String? QuickSearchText { get; set; } = "feature launch";
    public String? SearchText { get; set; } = "content search";
    public Guid? SelectedContentTypeId { get; set; } = ContentTypeArticleId;
    public DateOnly? PublishDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public Int32? Priority { get; set; } = 3;
    public String SelectedSort { get; set; } = "Updated";
    public Boolean? SortAscending { get; set; } = false;
    public Guid? SelectedWorkflowId { get; set; } = WorkflowDraftId;
    public Guid? PreviewContentStatusId { get; set; } = ContentStatusIds.Draft;

    public List<Guid?> SelectedChannelIds { get; set; } =
    [
        ChannelWebsiteId,
        ChannelDashboardId,
    ];

    public Boolean? ShowInNavigation { get; set; } = true;
    public Boolean? IsFeatured { get; set; } = true;
    public Boolean? IsSearchable { get; set; } = true;
    public String? HighlightColor { get; set; } = "#1c5f92";
    public String? ContentTitle { get; set; } = "Featured Content Collection";
    public String? Summary { get; set; } = "Use this preview to see how filters, forms, cards, content, alerts, and reports will feel in the target portal.";
    public String? EditableRichText { get; set; } =
        """
        <h2>Editor sample</h2>
        <p>This sample shows how the editable HTML surface sits against the active portal theme.</p>
        <p><strong>Bold text</strong>, <em>italic text</em>, and <a href="#">links</a> should remain easy to read without collapsing into the page background.</p>
        """;
    public Guid? TargetPortalId => Id ?? Path.Id("portal");

    public ObservableCollection<Selectable> AudienceOptions { get; } =
    [
        new() { Id = AudienceFamiliesId, Name = "Families", Selected = true },
        new() { Id = AudienceEducatorsId, Name = "Educators", Selected = true },
        new() { Id = AudiencePartnersId, Name = "Partners", Selected = false },
    ];

    public String FormattedHtml =>
        """
        <h1>Heading One</h1>
        <p>Body copy can include <strong>bold text</strong>, <em>italic text</em>, <u>underlined text</u>, and <a href="#">sample hyperlinks</a>.</p>
        <h2>Heading Two</h2>
        <p>Secondary paragraphs help demonstrate spacing, rhythm, and readable line height.</p>
        <h3>Heading Three</h3>
        <ul>
            <li>Unordered lists show bullet styling.</li>
            <li>Spacing between items should stay calm and legible.</li>
        </ul>
        <h4>Heading Four</h4>
        <ol>
            <li>Ordered lists help explain sequence.</li>
            <li>Quoted notes can stand apart from body copy.</li>
        </ol>
        <blockquote>
            <p>This quote sample helps show how formatted user HTML is framed inside the active portal theme.</p>
        </blockquote>
        """;

    public String SupportingHtml =>
        """
        <p>This labeled sample shows how <strong>supporting text</strong>, <em>editor-authored emphasis</em>, and <a href="#">inline links</a> will look beside forms and cards.</p>
        """;

    public String ReadOnlyRichText =>
        """
        <p>This read-only sample should stay on a distinct authoring surface instead of blending into the page behind it.</p>
        <p>Locked HTML needs to feel related to edit mode, not like a different layer of the portal.</p>
        """;

    private IEnumerable<PreviewAlert> PreviewAlerts =>
    [
        new("success", "c-icon-check", "Success alerts help users confirm a saved change."),
        new("warning", "c-icon-warning", "Warning alerts show important reminders."),
        new("error", "c-icon-cross", "Error alerts call attention to problems clearly."),
        new("tip", "c-icon-bulb", "Tips can feel friendly and helpful."),
        new("lock", "c-icon-lock", "Lock alerts explain restricted areas."),
    ];

    private IEnumerable<PreviewContentStatus> PreviewContentStatuses =>
    [
        new(ContentStatusIds.Draft, "Draft"),
        new(ContentStatusIds.Complete, "Complete"),
        new(ContentStatusIds.Retired, "Retired"),
    ];

    private IEnumerable<Named> PreviewContentStatusOptions =>
        PreviewContentStatuses.Select(x => new Named { Id = x.Id, Name = x.Name });

    private PreviewContentStatus ActivePreviewContentStatus =>
        PreviewContentStatuses.FirstOrDefault(x => x.Id.Equals(PreviewContentStatusId))
        ?? PreviewContentStatuses.First();

    private static IEnumerable<Named> ChannelOptions =>
    [
        new() { Id = ChannelWebsiteId, Name = "Website" },
        new() { Id = ChannelEmailId, Name = "Email" },
        new() { Id = ChannelDashboardId, Name = "Dashboard" },
    ];

    private static IEnumerable<Named> ContentTypeOptions =>
    [
        new() { Id = ContentTypeArticleId, Name = "Article" },
        new() { Id = ContentTypeCollectionId, Name = "Collection" },
        new() { Id = ContentTypePromoId, Name = "Promo Block" },
    ];

    private static IEnumerable<String> SortOptions =>
    [
        "Updated",
        "Title",
        "Type",
    ];

    private static IEnumerable<Named> WorkflowOptions =>
    [
        new() { Id = WorkflowDraftId, Name = "Draft" },
        new() { Id = WorkflowReviewId, Name = "In Review" },
        new() { Id = WorkflowPublishedId, Name = "Published" },
    ];

    protected override void OnInitialized()
    {
        EventBus.Subscribe(this);
    }

    public void Dispose()
    {
        EventBus.Unsubscribe(this);
    }

    public Task Handle(StyleSaved payload) => Refresh(payload.ContentPortalId);
    public Task Handle(FontSaved payload) => Refresh(payload.ContentPortalId);
    public Task Handle(FontRemoved payload) => Refresh(payload.ContentPortalId);

    private Task Refresh(Guid? portalId)
    {
        if (!portalId.Equals(TargetPortalId))
            return Task.CompletedTask;

        PreviewVersion++;
        return InvokeAsync(StateHasChanged);
    }

    private static Task HandlePreviewButtonClicked()
    {
        return Task.CompletedTask;
    }
}