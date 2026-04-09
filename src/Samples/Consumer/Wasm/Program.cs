using Crudspa.Samples.Consumer.Client;
using Crudspa.Framework.Core.Client.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

namespace Crudspa.Samples.Consumer.Wasm;

public class Program
{
    public static async Task Main(String[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.RootComponents.Add<RootRecover<ConsumerApp>>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddRadzenComponents();

        Registry.RegisterServices(builder.Services);

        builder.Services.AddScoped(_ => new HttpClient
        {
            BaseAddress = new(builder.HostEnvironment.BaseAddress),
            Timeout = TimeSpan.FromHours(4),
        });

        var host = builder.Build();

        await host.RunAsync();
    }
}