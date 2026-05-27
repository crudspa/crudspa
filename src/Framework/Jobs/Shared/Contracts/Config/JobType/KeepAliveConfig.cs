namespace Crudspa.Framework.Jobs.Shared.Contracts.Config.JobType;

public class KeepAliveConfig : Observable
{
    public String? Urls
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 ExpectedMaxLatencyMs
    {
        get;
        set => SetProperty(ref field, value);
    } = 5000;

    public List<String> GetUrls()
    {
        return Urls?
            .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList() ?? [];
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            var urls = GetUrls();

            if (urls.IsEmpty())
                errors.AddError("At least one URL is required.", nameof(Urls));

            if (ExpectedMaxLatencyMs < 100 || ExpectedMaxLatencyMs > 120000)
                errors.AddError("Expected Max Latency must be between 100 and 120000 milliseconds.", nameof(ExpectedMaxLatencyMs));

            foreach (var url in urls)
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme is not "http" and not "https")
                    errors.AddError($"URL '{url}' must be an absolute http or https URL.", nameof(Urls));
            }
        });
    }
}