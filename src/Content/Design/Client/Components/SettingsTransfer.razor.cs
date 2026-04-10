namespace Crudspa.Content.Design.Client.Components;

public partial class SettingsTransfer
{
    private Int32 _copyFeedbackVersion;
    private Int32 _pasteFeedbackVersion;

    [Parameter, EditorRequired] public String Description { get; set; } = null!;
    [Parameter, EditorRequired] public ModalModel HostModel { get; set; } = null!;
    [Parameter] public BoxModel? BoxModel { get; set; }
    [Parameter] public ItemModel? ItemModel { get; set; }
    [Parameter] public ContainerModel? ContainerModel { get; set; }

    [Inject] public IJsBridge JsBridge { get; set; } = null!;

    public Boolean CopyConfirmed { get; private set; }
    public Boolean PasteConfirmed { get; private set; }

    public String CopyIconClass => CopyConfirmed ? "c-icon-check" : "c-icon-copy";
    public String PasteIconClass => PasteConfirmed ? "c-icon-check" : "c-icon-clipboard";

    private async Task CopySettings()
    {
        var description = Description.HasSomething() ? Description : "Default";

        await JsBridge.CopyToClipboard(description);
        await FlashCopyConfirmation();
    }

    private async Task PasteSettings()
    {
        HostModel.Alerts.RemoveWhere(x => x.Dismissible);

        var clipboardText = await JsBridge.ReadFromClipboard();
        if (clipboardText is null)
        {
            await FlashPasteConfirmation();
            return;
        }

        SettingsTransferModel.TryApplyDescription(clipboardText, BoxModel, ItemModel, ContainerModel);

        await FlashPasteConfirmation();
    }

    private async Task FlashCopyConfirmation()
    {
        CopyConfirmed = true;
        var version = ++_copyFeedbackVersion;
        await InvokeAsync(StateHasChanged);
        await Task.Delay(TimeSpan.FromSeconds(1));

        if (version != _copyFeedbackVersion)
            return;

        CopyConfirmed = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task FlashPasteConfirmation()
    {
        PasteConfirmed = true;
        var version = ++_pasteFeedbackVersion;
        await InvokeAsync(StateHasChanged);
        await Task.Delay(TimeSpan.FromSeconds(1));

        if (version != _pasteFeedbackVersion)
            return;

        PasteConfirmed = false;
        await InvokeAsync(StateHasChanged);
    }
}