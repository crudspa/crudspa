using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class HtmlEditor
{
    [Parameter, EditorRequired] public String? Value { get; set; }
    [Parameter] public EventCallback<String?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public String EditorHeight { get; set; } = "24em";
    [Parameter] public String? DisplayHeight { get; set; }
    [Parameter] public Boolean AllowImages { get; set; }
    [Parameter] public G.List<String> Tokens { get; set; } = [];

    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public RadzenHtmlEditor EditorComponent { get; set; } = null!;
    public TokenModel InsertTokenModel { get; set; } = null!;
    public String? EditorValue => HtmlEditorMarkup.NormalizeForStorage(Value, AllowImages);

    protected override Task OnInitializedAsync()
    {
        InsertTokenModel = new(ScrollService);
        return Task.CompletedTask;
    }

    protected override Task OnParametersSetAsync()
    {
        if (Tokens.HasItems() && InsertTokenModel.SelectedToken.HasNothing())
            InsertTokenModel.SelectedToken = Tokens.First();

        return base.OnParametersSetAsync();
    }

    private async Task AddToken()
    {
        await EditorComponent.RestoreSelectionAsync();
        await EditorComponent.ExecuteCommandAsync(HtmlEditorCommands.InsertHtml, InsertTokenModel.SelectedToken);
        await InsertTokenModel.Hide();
    }

    private async Task HandleExecute(HtmlEditorExecuteEventArgs args)
    {
        if (args.CommandName == "InsertToken")
        {
            await args.Editor.SaveSelectionAsync();
            await InsertTokenModel.Show();
        }
    }

    private async Task HandleValueChanged(String? value)
    {
        value = HtmlEditorMarkup.NormalizeForStorage(value, AllowImages);
        await ValueChanged.InvokeAsync(value);
    }

    private void HandlePaste(HtmlEditorPasteEventArgs args)
    {
        if (args.Html.HasNothing())
            return;

        args.Html = HtmlEditorMarkup.NormalizeForPaste(args.Html, AllowImages);
    }

    public class TokenModel(IScrollService scrollService) : ModalModel(scrollService)
    {
        public String? SelectedToken
        {
            get;
            set => SetProperty(ref field, value);
        }
    }
}