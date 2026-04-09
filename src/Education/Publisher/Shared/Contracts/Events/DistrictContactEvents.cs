namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class DistrictContactPayload
{
    public Guid? Id { get; set; }
    public Guid? DistrictId { get; set; }
}

public class DistrictContactAdded : DistrictContactPayload;

public class DistrictContactSaved : DistrictContactPayload;

public class DistrictContactRemoved : DistrictContactPayload;