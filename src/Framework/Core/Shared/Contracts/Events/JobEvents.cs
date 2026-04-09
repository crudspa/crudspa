namespace Crudspa.Framework.Core.Shared.Contracts.Events;

public class JobPayload
{
    public Guid? Id { get; set; }
}

public class JobAdded : JobPayload;

public class JobSaved : JobPayload;

public class JobRemoved : JobPayload;