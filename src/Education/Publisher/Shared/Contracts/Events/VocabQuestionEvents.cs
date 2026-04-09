namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class VocabQuestionPayload
{
    public Guid? Id { get; set; }
    public Guid? VocabPartId { get; set; }
}

public class VocabQuestionAdded : VocabQuestionPayload;

public class VocabQuestionSaved : VocabQuestionPayload;

public class VocabQuestionRemoved : VocabQuestionPayload;

public class VocabQuestionsReordered : VocabQuestionPayload;