using HtmlEditorMarkup = Crudspa.Framework.Core.Client.Components.HtmlEditorMarkup;

namespace Crudspa.Content.Display.Client.Models;

public class QuestionDisplayModel : ScreenModel, IHandle<ValidateBinderElements>
{
    private readonly IEventBus _eventBus;
    private readonly IElementProgressService _elementProgressService;
    private readonly Guid? _surveyReplyId;
    private readonly Question? _question;
    private readonly QuestionReply? _initialReply;
    private String? _submittedSnapshot;

    public QuestionDisplayModel(IEventBus eventBus,
        IElementProgressService elementProgressService,
        ElementDisplayModel elementModel)
    {
        _eventBus = eventBus;
        _elementProgressService = elementProgressService;
        ElementModel = elementModel;
        QuestionElement = elementModel.RequireConfig<QuestionElement>();

        _eventBus.Subscribe(this);
    }

    public QuestionDisplayModel(IEventBus eventBus,
        IElementProgressService elementProgressService,
        Question question,
        Guid? surveyReplyId,
        QuestionReply? initialReply = null)
    {
        _eventBus = eventBus;
        _elementProgressService = elementProgressService;
        _question = question;
        _surveyReplyId = surveyReplyId;
        _initialReply = initialReply;

        _eventBus.Subscribe(this);
    }

    public ElementDisplayModel? ElementModel { get; }
    public QuestionElement? QuestionElement { get; }
    public Question Question => QuestionElement?.Question ?? _question!;

    public QuestionReply Reply { get; set => SetProperty(ref field, value); } = new();

    public Boolean IsSubmitted
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Initialize()
    {
        Question.EnsureAnswer();

        Reply = _initialReply?.DeepClone() ?? new();
        PrepareReply();

        if (ElementModel is null)
        {
            CaptureSubmittedSnapshot();
            return;
        }

        await ElementModel.InitializeProgress();

        var response = await _elementProgressService.FetchQuestionReply(new(new() { Id = ElementModel.Element.Id }));
        if (response.Ok && response.Value is not null)
            Reply = response.Value.DeepClone();

        PrepareReply();
        CaptureSubmittedSnapshot();
    }

    public async Task Handle(ValidateBinderElements payload)
    {
        if (Waiting
            || ElementModel is null
            || IsSubmittedUnchanged()
            || (ElementModel.Element.RequireInteraction != true && !HasReplyContent()))
            return;

        await Save();
    }

    public async Task Save()
    {
        Alerts.RemoveWhere(x => x.Dismissible);

        var errors = ValidateReply();
        if (errors.HasItems())
        {
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Error,
                Errors = errors,
            });

            if (ElementModel is not null)
            {
                ElementModel.MarkElementIncorrect();
                await _eventBus.Publish(new ValidateBinder());
            }

            return;
        }

        PrepareReply();

        var response = await WithWaiting("Submitting...", () => _elementProgressService.AddQuestionReply(new(Reply)));

        if (response.Ok)
        {
            IsSubmitted = true;
            _submittedSnapshot = CreateSubmittedSnapshot();

            if (ElementModel is not null)
                await ElementModel.MarkElementCompleted();
        }
    }

    public override void Dispose()
    {
        _eventBus.Unsubscribe(this);
        base.Dispose();
    }

    public void SetBoolean(Boolean? value)
    {
        Reply.BoolValue = value;
        RaisePropertyChanged(nameof(Reply));
    }

    public Boolean IsChoiceSelected(OptionsAnswerChoice choice) =>
        Reply.AnswerChoices.Any(x => x.ChoiceId == choice.Id);

    public void SetSingleChoice(OptionsAnswerChoice choice)
    {
        Reply.AnswerChoices.Clear();
        Reply.AnswerChoices.Add(new()
        {
            ChoiceId = choice.Id,
            Choice = choice,
        });
    }

    public void ToggleChoice(OptionsAnswerChoice choice, Boolean selected)
    {
        Reply.AnswerChoices.RemoveWhere(x => x.ChoiceId == choice.Id);

        if (selected)
        {
            Reply.AnswerChoices.Add(new()
            {
                ChoiceId = choice.Id,
                Choice = choice,
            });
        }
    }

    public IList<ScaleOption> ScaleOptions()
    {
        if (Question.ScaleAnswer is not { } answer)
            return [];

        if (answer.Kind == ScaleAnswer.Kinds.Rating)
        {
            var min = answer.RatingMin.GetValueOrDefault(1);
            var max = answer.RatingMax.GetValueOrDefault(5);

            if (max < min)
                (min, max) = (max, min);

            return Enumerable.Range(min, max - min + 1)
                .Select(value => new ScaleOption(value, answer.RatingKind == ScaleAnswer.RatingKinds.Stars ? "Stars " + value : value.ToString()))
                .ToList();
        }

        var options = answer.LikertKind switch
        {
            ScaleAnswer.LikertKinds.Frequency => new[]
            {
                new ScaleOption(5, "Always"),
                new ScaleOption(4, "Often"),
                new ScaleOption(3, "Sometimes"),
                new ScaleOption(2, "Rarely"),
                new ScaleOption(1, "Never"),
            },
            ScaleAnswer.LikertKinds.Importance => new[]
            {
                new ScaleOption(5, "Very Important"),
                new ScaleOption(4, "Important"),
                new ScaleOption(3, "Moderately Important"),
                new ScaleOption(2, "Slightly Important"),
                new ScaleOption(1, "Unimportant"),
            },
            ScaleAnswer.LikertKinds.Quality => new[]
            {
                new ScaleOption(5, "Excellent"),
                new ScaleOption(4, "Good"),
                new ScaleOption(3, "Fair"),
                new ScaleOption(2, "Poor"),
                new ScaleOption(1, "Very Poor"),
            },
            ScaleAnswer.LikertKinds.Likelihood => new[]
            {
                new ScaleOption(5, "Definitely"),
                new ScaleOption(4, "Probably"),
                new ScaleOption(3, "Possibly"),
                new ScaleOption(2, "Probably Not"),
                new ScaleOption(1, "Definitely Not"),
            },
            _ => new[]
            {
                new ScaleOption(5, "Strongly Agree"),
                new ScaleOption(4, "Agree"),
                new ScaleOption(3, "Undecided"),
                new ScaleOption(2, "Disagree"),
                new ScaleOption(1, "Strongly Disagree"),
            },
        };

        return answer.Ordering == ScaleAnswer.Orderings.LowToHigh
            ? options.Reverse().ToList()
            : options.ToList();
    }

    private List<Error> ValidateReply()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Question.AnswerTypeId == AnswerTypeIds.Boolean)
                ValidateBoolean(errors);
            else if (Question.AnswerTypeId == AnswerTypeIds.Contact)
                ValidateContact(errors);
            else if (Question.AnswerTypeId == AnswerTypeIds.Date)
                ValidateDate(errors);
            else if (Question.AnswerTypeId == AnswerTypeIds.File)
                ValidateFile(errors);
            else if (Question.AnswerTypeId == AnswerTypeIds.Number)
                ValidateNumber(errors);
            else if (Question.AnswerTypeId == AnswerTypeIds.Options)
                ValidateOptions(errors);
            else if (Question.AnswerTypeId == AnswerTypeIds.Scale)
                ValidateScale(errors);
            else
                ValidateText(errors);
        });
    }

    private void ValidateBoolean(List<Error> errors)
    {
        if (Question.BooleanAnswer?.Kind == BooleanAnswer.Kinds.Checkbox)
        {
            if (Reply.BoolValue != true)
                errors.AddError("Please check the box to continue.", nameof(Reply.BoolValue));
        }
        else if (!Reply.BoolValue.HasValue)
            errors.AddError("Please choose an answer.", nameof(Reply.BoolValue));
    }

    private void ValidateContact(List<Error> errors)
    {
        var answer = Question.ContactAnswer;
        if (answer is null)
            return;

        if (answer.Kind == ContactAnswer.Kinds.Postal)
        {
            errors.AddRange(Reply.Postal.Validate());
            return;
        }

        if (Reply.TextValue.HasNothing())
        {
            errors.AddError("Please enter a response.", nameof(Reply.TextValue));
            return;
        }

        if (answer.Kind == ContactAnswer.Kinds.Email && !Reply.TextValue!.Contains('@'))
            errors.AddError("Please enter a valid email address.", nameof(Reply.TextValue));
    }

    private void ValidateDate(List<Error> errors)
    {
        var answer = Question.DateAnswer;
        if (answer is null)
            return;

        if (answer.Kind == DateAnswer.Kinds.Date)
        {
            if (!Reply.DateValue.HasValue)
                errors.AddError("Please choose a date.", nameof(Reply.DateValue));
            else
            {
                if (answer.DateMin.HasValue && Reply.DateValue < answer.DateMin)
                    errors.AddError("Date is before the allowed minimum.", nameof(Reply.DateValue));
                if (answer.DateMax.HasValue && Reply.DateValue > answer.DateMax)
                    errors.AddError("Date is after the allowed maximum.", nameof(Reply.DateValue));
            }
        }
        else if (answer.Kind == DateAnswer.Kinds.Time)
        {
            if (!Reply.TimeValue.HasValue)
                errors.AddError("Please choose a time.", nameof(Reply.TimeValue));
            else
            {
                if (answer.TimeMin.HasValue && Reply.TimeValue < answer.TimeMin)
                    errors.AddError("Time is before the allowed minimum.", nameof(Reply.TimeValue));
                if (answer.TimeMax.HasValue && Reply.TimeValue > answer.TimeMax)
                    errors.AddError("Time is after the allowed maximum.", nameof(Reply.TimeValue));
            }
        }
        else if (!Reply.DateTimeValue.HasValue)
            errors.AddError("Please choose a date and time.", nameof(Reply.DateTimeValue));
        else
        {
            if (answer.DateTimeMin.HasValue && Reply.DateTimeValue < answer.DateTimeMin)
                errors.AddError("Date/time is before the allowed minimum.", nameof(Reply.DateTimeValue));
            if (answer.DateTimeMax.HasValue && Reply.DateTimeValue > answer.DateTimeMax)
                errors.AddError("Date/time is after the allowed maximum.", nameof(Reply.DateTimeValue));
        }
    }

    private void ValidateFile(List<Error> errors)
    {
        var hasFile = Question.FileAnswer?.Kind switch
        {
            FileAnswer.Kinds.Audio => Reply.AudioFile.BlobId.HasValue || Reply.AudioFile.Id.HasValue,
            FileAnswer.Kinds.Image => Reply.ImageFile.BlobId.HasValue || Reply.ImageFile.Id.HasValue,
            FileAnswer.Kinds.Pdf => Reply.PdfFile.BlobId.HasValue || Reply.PdfFile.Id.HasValue,
            FileAnswer.Kinds.Video => Reply.VideoFile.BlobId.HasValue || Reply.VideoFile.Id.HasValue,
            _ => false,
        };

        if (!hasFile)
            errors.AddError("Please upload a file.");
    }

    private void ValidateNumber(List<Error> errors)
    {
        var answer = Question.NumberAnswer;
        if (answer is null)
            return;

        if (answer.Kind == NumberAnswer.Kinds.Integer)
        {
            if (!Reply.IntegerValue.HasValue)
                errors.AddError("Please enter a number.", nameof(Reply.IntegerValue));
            else
            {
                if (answer.IntegerMin.HasValue && Reply.IntegerValue < answer.IntegerMin)
                    errors.AddError("Number is below the allowed minimum.", nameof(Reply.IntegerValue));
                if (answer.IntegerMax.HasValue && Reply.IntegerValue > answer.IntegerMax)
                    errors.AddError("Number is above the allowed maximum.", nameof(Reply.IntegerValue));
            }
        }
        else
        {
            var value = answer.Kind == NumberAnswer.Kinds.Currency ? Reply.CurrencyValue : Reply.DecimalValue;
            var min = answer.Kind == NumberAnswer.Kinds.Currency ? answer.CurrencyMin : answer.DecimalMin;
            var max = answer.Kind == NumberAnswer.Kinds.Currency ? answer.CurrencyMax : answer.DecimalMax;

            if (!value.HasValue)
                errors.AddError("Please enter a number.", answer.Kind == NumberAnswer.Kinds.Currency ? nameof(Reply.CurrencyValue) : nameof(Reply.DecimalValue));
            else
            {
                if (min.HasValue && value < min)
                    errors.AddError("Number is below the allowed minimum.");
                if (max.HasValue && value > max)
                    errors.AddError("Number is above the allowed maximum.");
            }
        }
    }

    private void ValidateOptions(List<Error> errors)
    {
        var answer = Question.OptionsAnswer;
        if (answer is null)
            return;

        var selected = Reply.AnswerChoices.Count + (Reply.OtherBoolValue == true && Reply.OtherTextValue.HasSomething() ? 1 : 0);
        var minimum = answer.Kind == OptionsAnswer.Kinds.Multiple ? answer.MinSelections.GetValueOrDefault(1) : 1;
        var maximum = answer.Kind == OptionsAnswer.Kinds.Single ? 1 : answer.MaxSelections;

        if (selected < minimum)
            errors.AddError(minimum == 1 ? "Please choose an answer." : $"Please choose at least {minimum} answers.");

        if (maximum.HasValue && selected > maximum)
            errors.AddError($"Please choose no more than {maximum} answers.");

        if (Reply.OtherBoolValue == true && Reply.OtherTextValue.HasNothing())
            errors.AddError("Please enter an other response.", nameof(Reply.OtherTextValue));
    }

    private void ValidateScale(List<Error> errors)
    {
        if (!Reply.IntegerValue.HasValue)
            errors.AddError("Please choose a rating.", nameof(Reply.IntegerValue));
    }

    private void ValidateText(List<Error> errors)
    {
        if (Question.TextAnswer?.Kind == TextAnswer.Kinds.Rich)
        {
            Reply.HtmlValue = HtmlEditorMarkup.NormalizeForStorage(Reply.HtmlValue);
            if (Reply.HtmlValue.HasNothing())
                errors.AddError("Please enter a response.", nameof(Reply.HtmlValue));
        }
        else if (Reply.TextValue.HasNothing())
            errors.AddError("Please enter a response.", nameof(Reply.TextValue));
    }

    private void PrepareReply()
    {
        Reply.ElementId = ElementModel?.Element.Id;
        Reply.SurveyReplyId = _surveyReplyId ?? Reply.SurveyReplyId;
        Reply.QuestionId = Question.Id;
        Reply.Question = Question;
        Reply.Submitted = DateTimeOffset.Now;

        if (Question.TextAnswer?.Kind == TextAnswer.Kinds.Rich)
            Reply.HtmlValue = HtmlEditorMarkup.NormalizeForStorage(Reply.HtmlValue);
    }

    private void CaptureSubmittedSnapshot()
    {
        IsSubmitted = HasReplyContent();
        _submittedSnapshot = IsSubmitted ? CreateSubmittedSnapshot() : null;
    }

    private Boolean IsSubmittedUnchanged() =>
        IsSubmitted && _submittedSnapshot.IsBasically(CreateSubmittedSnapshot());

    private String CreateSubmittedSnapshot()
    {
        var postal = Reply.Postal;

        return new
        {
            Reply.BoolValue,
            Reply.TextValue,
            HtmlValue = HtmlEditorMarkup.NormalizeForStorage(Reply.HtmlValue),
            Reply.DateValue,
            Reply.TimeValue,
            Reply.DateTimeValue,
            Reply.IntegerValue,
            Reply.DecimalValue,
            Reply.CurrencyValue,
            Reply.OtherBoolValue,
            Reply.OtherTextValue,
            Reply.AudioId,
            Reply.ImageId,
            Reply.PdfId,
            Reply.VideoId,
            Reply.PostalId,
            AudioFileId = Reply.AudioFile.Id,
            AudioFileBlobId = Reply.AudioFile.BlobId,
            ImageFileId = Reply.ImageFile.Id,
            ImageFileBlobId = Reply.ImageFile.BlobId,
            PdfFileId = Reply.PdfFile.Id,
            PdfFileBlobId = Reply.PdfFile.BlobId,
            VideoFileId = Reply.VideoFile.Id,
            VideoFileBlobId = Reply.VideoFile.BlobId,
            Postal = new
            {
                postal.Id,
                postal.RecipientName,
                postal.BusinessName,
                postal.StreetAddress,
                postal.City,
                postal.StateId,
                postal.PostalCode,
            },
            ChoiceIds = Reply.AnswerChoices
                .Select(x => x.ChoiceId)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .OrderBy(x => x)
                .ToList(),
        }.ToJson() ?? String.Empty;
    }

    private Boolean HasReplyContent() =>
        Reply.BoolValue.HasValue
        || Reply.TextValue.HasSomething()
        || HtmlEditorMarkup.NormalizeForStorage(Reply.HtmlValue).HasSomething()
        || Reply.DateValue.HasValue
        || Reply.TimeValue.HasValue
        || Reply.DateTimeValue.HasValue
        || Reply.IntegerValue.HasValue
        || Reply.DecimalValue.HasValue
        || Reply.CurrencyValue.HasValue
        || Reply.OtherBoolValue.HasValue
        || Reply.OtherTextValue.HasSomething()
        || Reply.AudioId.HasValue
        || Reply.ImageId.HasValue
        || Reply.PdfId.HasValue
        || Reply.VideoId.HasValue
        || Reply.PostalId.HasValue
        || Reply.AudioFile.Id.HasValue
        || Reply.AudioFile.BlobId.HasValue
        || Reply.ImageFile.Id.HasValue
        || Reply.ImageFile.BlobId.HasValue
        || Reply.PdfFile.Id.HasValue
        || Reply.PdfFile.BlobId.HasValue
        || Reply.VideoFile.Id.HasValue
        || Reply.VideoFile.BlobId.HasValue
        || Reply.AnswerChoices.Any()
        || HasPostalContent();

    private Boolean HasPostalContent() =>
        Reply.Postal.RecipientName.HasSomething()
        || Reply.Postal.BusinessName.HasSomething()
        || Reply.Postal.StreetAddress.HasSomething()
        || Reply.Postal.City.HasSomething()
        || Reply.Postal.StateId.HasValue
        || Reply.Postal.PostalCode.HasSomething();
}

public record ScaleOption(Int32 Value, String Label);