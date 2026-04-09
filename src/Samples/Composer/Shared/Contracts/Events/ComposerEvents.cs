namespace Crudspa.Samples.Composer.Shared.Contracts.Events;

public class ComposerPayload
{
    public Guid? Id { get; set; }
}

public class ComposerAdded : ComposerPayload;

public class ComposerSaved : ComposerPayload;

public class ComposerRemoved : ComposerPayload;