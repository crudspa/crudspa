namespace Crudspa.Framework.Jobs.Shared.Contracts.Events;

public class JobSchedulePayload
{
    public Guid? Id { get; set; }
}

public class JobScheduleAdded : JobSchedulePayload { }

public class JobScheduleSaved : JobSchedulePayload { }

public class JobScheduleRemoved : JobSchedulePayload { }