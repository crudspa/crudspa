using Crudspa.Content.Display.Shared.Contracts.Config.ElementType;

namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class QuestionReply : Observable
{
    public Guid? Id { get; set => SetProperty(ref field, value); }
    public Guid? ElementId { get; set => SetProperty(ref field, value); }
    public Guid? SurveyReplyId { get; set => SetProperty(ref field, value); }
    public Guid? QuestionId { get; set => SetProperty(ref field, value); }
    public Question? Question { get; set => SetProperty(ref field, value); }
    public DateTimeOffset? Submitted { get; set => SetProperty(ref field, value); }
    public Boolean? BoolValue { get; set => SetProperty(ref field, value); }
    public String? TextValue { get; set => SetProperty(ref field, value); }
    public String? HtmlValue { get; set => SetProperty(ref field, value); }
    public DateOnly? DateValue { get; set => SetProperty(ref field, value); }
    public TimeOnly? TimeValue { get; set => SetProperty(ref field, value); }
    public DateTimeOffset? DateTimeValue { get; set => SetProperty(ref field, value); }
    public Int32? IntegerValue { get; set => SetProperty(ref field, value); }
    public Single? DecimalValue { get; set => SetProperty(ref field, value); }
    public Single? CurrencyValue { get; set => SetProperty(ref field, value); }
    public Boolean? OtherBoolValue { get; set => SetProperty(ref field, value); }
    public String? OtherTextValue { get; set => SetProperty(ref field, value); }
    public Guid? AudioId { get; set => SetProperty(ref field, value); }
    public Guid? ImageId { get; set => SetProperty(ref field, value); }
    public Guid? PdfId { get; set => SetProperty(ref field, value); }
    public Guid? VideoId { get; set => SetProperty(ref field, value); }
    public Guid? PostalId { get; set => SetProperty(ref field, value); }
    public AudioFile AudioFile { get; set => SetProperty(ref field, value); } = new();
    public ImageFile ImageFile { get; set => SetProperty(ref field, value); } = new();
    public PdfFile PdfFile { get; set => SetProperty(ref field, value); } = new();
    public VideoFile VideoFile { get; set => SetProperty(ref field, value); } = new();
    public UsaPostal Postal { get; set => SetProperty(ref field, value); } = new();
    public ObservableCollection<AnswerChoiceReply> AnswerChoices { get; set => SetProperty(ref field, value); } = [];
}

public class AnswerChoiceReply : Observable
{
    public Guid? Id { get; set => SetProperty(ref field, value); }
    public Guid? QuestionReplyId { get; set => SetProperty(ref field, value); }
    public Guid? ChoiceId { get; set => SetProperty(ref field, value); }
    public OptionsAnswerChoice? Choice { get; set => SetProperty(ref field, value); }
}