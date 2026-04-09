namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class ReadQuestionPayload
{
    public Guid? Id { get; set; }
    public Guid? ReadPartId { get; set; }
}

public class ReadQuestionAdded : ReadQuestionPayload;

public class ReadQuestionSaved : ReadQuestionPayload;

public class ReadQuestionRemoved : ReadQuestionPayload;

public class ReadQuestionsReordered : ReadQuestionPayload;