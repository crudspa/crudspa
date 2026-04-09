namespace Crudspa.Samples.Catalog.Shared.Contracts.Events;

public class MoviePayload
{
    public Guid? Id { get; set; }
}

public class MovieAdded : MoviePayload;

public class MovieSaved : MoviePayload;

public class MovieRemoved : MoviePayload;