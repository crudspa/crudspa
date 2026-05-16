namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class SurveyPayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
}

public class SurveyAdded : SurveyPayload;

public class SurveySaved : SurveyPayload;

public class SurveyRemoved : SurveyPayload;

public class SurveyPartPayload
{
    public Guid? Id { get; set; }
    public Guid? SurveyId { get; set; }
}

public class SurveyPartAdded : SurveyPartPayload;

public class SurveyPartSaved : SurveyPartPayload;

public class SurveyPartRemoved : SurveyPartPayload;

public class SurveyPartsReordered : SurveyPartPayload;

public class SurveyQuestionPayload
{
    public Guid? Id { get; set; }
    public Guid? PartId { get; set; }
}

public class SurveyQuestionAdded : SurveyQuestionPayload;

public class SurveyQuestionSaved : SurveyQuestionPayload;

public class SurveyQuestionRemoved : SurveyQuestionPayload;

public class SurveyQuestionsReordered : SurveyQuestionPayload;