namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class AssessmentAssignment : Observable, IValidates, INamed, ICountable
{
    private String? _studentFirstName;
    private String? _studentLastName;

    public String Name => (_studentFirstName + " " + _studentLastName).Trim();

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssessmentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Assigned
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? StartAfter
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? EndBefore
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Started
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Completed
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Terminated
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AssessmentName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateOnly? AssessmentAvailableStart
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateOnly? AssessmentAvailableEnd
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile? ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? StudentFirstName
    {
        get => _studentFirstName;
        set => SetProperty(ref _studentFirstName, value);
    }

    public String? StudentLastName
    {
        get => _studentLastName;
        set => SetProperty(ref _studentLastName, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ListenPartCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ListenQuestionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ListenChoiceSelectionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ListenPartCompletedCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ListenTextEntryCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ReadPartCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ReadQuestionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ReadChoiceSelectionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ReadPartCompletedCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ReadTextEntryCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? VocabPartCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? VocabQuestionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? VocabChoiceSelectionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? VocabPartCompletedCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ListenPartCompleted> ListenPartCompleteds
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<ReadPartCompleted> ReadPartCompleteds
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<VocabPartCompleted> VocabPartCompleteds
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public String PartsProgress
    {
        get
        {
            if (ListenPartCompletedCount is null || ReadPartCompletedCount is null || VocabPartCompletedCount is null
                || ListenPartCount is null || ReadPartCount is null || VocabPartCount is null)

                return String.Empty;

            var completed = ListenPartCompletedCount + ReadPartCompletedCount + VocabPartCompletedCount;
            var total = ListenPartCount.Value + ReadPartCount + VocabPartCount;

            return $"{completed:N0} of {total:N0}";
        }
    }

    public String QuestionsProgress
    {
        get
        {
            if (ListenChoiceSelectionCount is null || ListenTextEntryCount is null || ReadChoiceSelectionCount is null || ReadTextEntryCount is null || VocabChoiceSelectionCount is null
                || ListenQuestionCount is null || ReadQuestionCount is null || VocabQuestionCount is null)

                return String.Empty;

            var completed = ListenChoiceSelectionCount + ListenTextEntryCount + ReadChoiceSelectionCount + ReadTextEntryCount + VocabChoiceSelectionCount;
            var total = ListenQuestionCount.Value + ReadQuestionCount + VocabQuestionCount;

            return $"{completed:N0} of {total:N0}";
        }
    }

    public void SetDefaultDates(String timeZoneId)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var userTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);

        var tomorrowMorning = userTime.AddDays(1).Date.AddHours(8);
        var twoMonthsHence = userTime.AddDays(1).Date.AddMonths(2).AddHours(16);

        StartAfter = new DateTimeOffset(tomorrowMorning, timeZone.GetUtcOffset(tomorrowMorning));
        EndBefore = new DateTimeOffset(twoMonthsHence, timeZone.GetUtcOffset(twoMonthsHence));
    }

    public virtual List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!AssessmentId.HasValue)
                errors.AddError("Assessment is required.", nameof(AssessmentId));

            if (!StudentId.HasValue)
                errors.AddError("Student is required.", nameof(StudentId));

            if (!StartAfter.HasValue)
                errors.AddError("Start After is required.", nameof(StartAfter));

            if (!EndBefore.HasValue)
                errors.AddError("End Before is required.", nameof(EndBefore));
        });
    }
}