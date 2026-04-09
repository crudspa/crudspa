namespace Crudspa.Education.Common.Shared.Contracts;

public enum AssessmentStates
{
    VocabPreview,
    VocabQuestions,
    ListenPassage,
    ListenPreview,
    ListenQuestions,
    ReadPreview,
    ReadQuestions,
}

public enum AssessmentPartStates { Accepting, Invalid, Valid }

public enum AssessmentQuestionStates { Invalid, Valid }

public enum AssessmentChoiceStates
{
    Default, Selected, Invalid, Valid,
}