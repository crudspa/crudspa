namespace Crudspa.Samples.Composer.Shared.Contracts.Events;

public class ComposerContactPayload
{
    public Guid? Id { get; set; }
}

public class ComposerContactAdded : ComposerContactPayload;

public class ComposerContactSaved : ComposerContactPayload;

public class ComposerContactRemoved : ComposerContactPayload;