namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class CommunityPayload
{
    public Guid? Id { get; set; }
    public Guid? DistrictId { get; set; }
}

public class CommunityAdded : CommunityPayload;

public class CommunitySaved : CommunityPayload;

public class CommunityRemoved : CommunityPayload;

public class CommunityRelationsSaved : CommunityPayload;