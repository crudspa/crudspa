namespace Crudspa.Education.District.Shared.Contracts.Events;

public class DistrictContactPayload
{
    public Guid? Id { get; set; }
}

public class DistrictContactAdded : DistrictContactPayload;

public class DistrictContactSaved : DistrictContactPayload;

public class DistrictContactRemoved : DistrictContactPayload;