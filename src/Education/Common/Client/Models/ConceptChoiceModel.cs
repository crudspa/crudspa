namespace Crudspa.Education.Common.Client.Models;

public class ConceptChoiceModel : Observable
{
    public ActivityChoice Choice { get; }

    public String FirstHtml
    {
        get;
        set => SetProperty(ref field, value);
    } = String.Empty;

    public String LastHtml
    {
        get;
        set => SetProperty(ref field, value);
    } = String.Empty;

    public DestinationModel? Destination
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String CssClass
    {
        get
        {
            return Destination!.State switch
            {
                DestinationModel.States.Empty => "empty",
                DestinationModel.States.Filled => "filled",
                DestinationModel.States.Valid => "valid",
                DestinationModel.States.Invalid => "invalid",
                _ => "empty",
            };
        }
    }

    public ConceptChoiceModel(ActivityChoice choice)
    {
        Choice = choice;

        var input = Choice.Text;
        var openBracketIndex = input!.IndexOf("[", StringComparison.Ordinal);

        if (openBracketIndex < 0)
            FirstHtml = ToHtml(input);
        else
        {
            var closeBracketIndex = input.IndexOf("]", openBracketIndex + 1, StringComparison.Ordinal);
            if (closeBracketIndex < 0)
                FirstHtml = ToHtml(input);
            else
            {
                FirstHtml = ToHtml(input.Substring(0, openBracketIndex));
                LastHtml = ToHtml(input.Substring(closeBracketIndex + 1));

                var clonedChoice = new ActivityChoice
                {
                    Id = choice.Id,
                    Text = input.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1),
                };

                Destination = new(clonedChoice, null);
            }
        }

        Destination ??= new(Choice, null);
    }

    private static String ToHtml(String input)
    {
        const String boldIndicator = "**";

        var openBoldIndex = input.IndexOf(boldIndicator, StringComparison.Ordinal);

        if (openBoldIndex < 0)
            return input;

        var closeBoldIndex = input.IndexOf(boldIndicator, openBoldIndex + boldIndicator.Length, StringComparison.Ordinal);

        if (closeBoldIndex < 0)
            return input;

        var beforeBold = input.Substring(0, openBoldIndex);
        var insideBold = input.Substring(openBoldIndex + boldIndicator.Length, closeBoldIndex - openBoldIndex - boldIndicator.Length);
        var afterBold = input.Substring(closeBoldIndex + boldIndicator.Length);

        return $"{beforeBold}<strong>{insideBold}</strong>{afterBold}";
    }
}