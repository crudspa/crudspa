using System.Text.Json.Serialization;

namespace Crudspa.Framework.Auth.Server.Contracts.Data;

public class AuthCompletion
{
    public enum Codes
    {
        Success = 0,
        InvalidTransaction = 1,
        AudienceMismatch = 2,
        IdentityNotFound = 3,
        LinkNotFound = 4,
        LinkAmbiguous = 5,
        InvalidIdentity = 6,
    }

    public Codes Code { get; set; }
    public Guid? PortalId { get; set; }
    public Guid? UserId { get; set; }
    public String? ReturnPath { get; set; }
    public String? Audience { get; set; }

    [JsonIgnore]
    public String? HandoffCode { get; set; }
}