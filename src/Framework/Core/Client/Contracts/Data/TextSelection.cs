using System.Text.Json.Serialization;

namespace Crudspa.Framework.Core.Client.Contracts.Data;

public class TextSelection
{
    [JsonPropertyName("start")]
    public Int32 Start { get; set; }

    [JsonPropertyName("end")]
    public Int32 End { get; set; }
}