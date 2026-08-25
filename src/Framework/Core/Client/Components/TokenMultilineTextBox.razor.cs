using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class TokenMultilineTextBox
{
    [Parameter] public String? Value { get; set; }
    [Parameter] public EventCallback<String?> ValueChanged { get; set; }
    [Parameter] public String? Label { get; set; }
    [Parameter] public Field.Sizes Size { get; set; } = Field.Sizes.Unspecified;
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public MultilineTextBox.Heights Height { get; set; } = MultilineTextBox.Heights.Unspecified;
    [Parameter] public Int32? MaxLength { get; set; }
    [Parameter] public String? Placeholder { get; set; }
    [Parameter] public G.List<String> Tokens { get; set; } = [];

    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public TokenModel InsertTokenModel { get; set; } = null!;
    public MultilineTextBox TextBox { get; set; } = null!;
    public G.IReadOnlyList<String> SortedTokens => Tokens.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();

    protected override Task OnInitializedAsync()
    {
        InsertTokenModel = new(ScrollService);
        return Task.CompletedTask;
    }

    protected override Task OnParametersSetAsync()
    {
        if (SortedTokens.HasItems() && InsertTokenModel.SelectedToken.HasNothing())
            InsertTokenModel.SelectedToken = SortedTokens.First();

        return base.OnParametersSetAsync();
    }

    public async Task InsertToken()
    {
        if (InsertTokenModel.SelectedToken.HasNothing())
            return;

        var token = InsertTokenModel.SelectedToken!;
        var value = await TextBox.InsertTextAtSelection(token);

        if (value is null)
        {
            Value = AppendToken(Value, token);
            await ValueChanged.InvokeAsync(Value);
        }

        await InsertTokenModel.Hide();
    }

    public async Task ShowInsertToken()
    {
        await TextBox.CaptureSelection();
        await InsertTokenModel.Show();
    }

    private async Task HandleValueChanged(String? value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(value);
    }

    private static String AppendToken(String? value, String token)
    {
        if (value.HasNothing())
            return token;

        return value!.EndsWith(' ') || value.EndsWith(Environment.NewLine)
            ? value + token
            : value + " " + token;
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