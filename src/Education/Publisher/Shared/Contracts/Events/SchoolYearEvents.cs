namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class SchoolYearPayload
{
    public Guid? Id { get; set; }
}

public class SchoolYearAdded : SchoolYearPayload;

public class SchoolYearSaved : SchoolYearPayload;

public class SchoolYearRemoved : SchoolYearPayload;