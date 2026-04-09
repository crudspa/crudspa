namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class ListenQuestionPayload
{
    public Guid? Id { get; set; }
    public Guid? ListenPartId { get; set; }
}

public class ListenQuestionAdded : ListenQuestionPayload;

public class ListenQuestionSaved : ListenQuestionPayload;

public class ListenQuestionRemoved : ListenQuestionPayload;

public class ListenQuestionsReordered : ListenQuestionPayload;