namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class TrifoldPayload
{
    public Guid? Id { get; set; }
    public Guid? BookId { get; set; }
}

public class TrifoldAdded : TrifoldPayload;

public class TrifoldSaved : TrifoldPayload;

public class TrifoldRemoved : TrifoldPayload;

public class TrifoldsReordered : TrifoldPayload;