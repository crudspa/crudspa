namespace Crudspa.Content.Display.Client.Plugins.AnswerType;

public partial class ScaleAnswerDisplay : IAnswerDisplay
{
    [Parameter] public QuestionDisplayModel Model { get; set; } = null!;

    public ScaleAnswer? Answer => Model.Question.ScaleAnswer;

    public String GroupName => $"scale-{Model.Question.Id}";

    public Boolean IsRatingNumbers => Answer is
    {
        Kind: ScaleAnswer.Kinds.Rating,
        RatingKind: ScaleAnswer.RatingKinds.Numbers,
    };

    public Boolean IsRatingStars => Answer is
    {
        Kind: ScaleAnswer.Kinds.Rating,
        RatingKind: ScaleAnswer.RatingKinds.Stars,
    };

    public Boolean IsSelected(ScaleOption option) => Model.Reply.IntegerValue == option.Value;

    public void Select(ScaleOption option) => Model.Reply.IntegerValue = option.Value;

    public String StarImage(ScaleOption option) =>
        Model.Reply.IntegerValue.GetValueOrDefault(Int32.MinValue) >= option.Value
            ? "/api/content/display/images/star-filled"
            : "/api/content/display/images/star-empty";

    public String StarLabel(ScaleOption option) =>
        option.Value == 1 ? "1 star" : $"{option.Value} stars";
}