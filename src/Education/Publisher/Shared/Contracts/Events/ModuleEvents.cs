namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class ModulePayload
{
    public Guid? Id { get; set; }
    public Guid? BookId { get; set; }
}

public class ModuleAdded : ModulePayload;

public class ModuleSaved : ModulePayload;

public class ModuleRemoved : ModulePayload;

public class ModulesReordered : ModulePayload;