namespace Crudspa.Education.District.Shared.Contracts.Events;

public class DistrictPayload
{
    public Guid? Id { get; set; }
}

public class DistrictAdded : DistrictPayload;

public class DistrictSaved : DistrictPayload;

public class DistrictRemoved : DistrictPayload;