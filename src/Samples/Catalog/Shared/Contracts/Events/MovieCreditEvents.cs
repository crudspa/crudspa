namespace Crudspa.Samples.Catalog.Shared.Contracts.Events;

public class MovieCreditPayload
{
    public Guid? Id { get; set; }
    public Guid? MovieId { get; set; }
}

public class MovieCreditAdded : MovieCreditPayload;

public class MovieCreditSaved : MovieCreditPayload;

public class MovieCreditRemoved : MovieCreditPayload;

public class MovieCreditsReordered : MovieCreditPayload;