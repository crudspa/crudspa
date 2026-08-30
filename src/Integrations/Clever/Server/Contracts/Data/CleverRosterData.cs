using System.Text.Json.Serialization;

namespace Crudspa.Integrations.Clever.Server.Contracts.Data;

internal class CleverIdentity
{
    public String? Sub { get; set; }
    [JsonPropertyName("district_id")] public String? DistrictId { get; set; }
    [JsonPropertyName("user_type")] public String? UserType { get; set; }
    [JsonPropertyName("authorized_by")] public String? AuthorizedBy { get; set; }
}

internal class CleverSchool
{
    public String Id { get; set; } = null!;
    [JsonPropertyName("sis_id")] public String? SisId { get; set; }
    public String Name { get; set; } = null!;
}

internal class CleverTerm
{
    public String Id { get; set; } = null!;
    public String? Name { get; set; }
    [JsonPropertyName("start_date")] public DateOnly? Starts { get; set; }
    [JsonPropertyName("end_date")] public DateOnly? Ends { get; set; }
}

internal class CleverCourse
{
    public String Id { get; set; } = null!;
    public String? Name { get; set; }
    public String? Number { get; set; }
}

internal class CleverSection
{
    public String Id { get; set; } = null!;
    [JsonPropertyName("sis_id")] public String? SisId { get; set; }
    public String School { get; set; } = null!;
    public String? Course { get; set; }
    [JsonPropertyName("term_id")] public String? TermId { get; set; }
    public String Name { get; set; } = null!;
    public String? Grade { get; set; }
    public String? Subject { get; set; }
    public IList<String> Students { get; set; } = [];
    public String? Teacher { get; set; }
    public IList<String> Teachers { get; set; } = [];
}

internal class CleverUser
{
    public String Id { get; set; } = null!;
    public String? Email { get; set; }
    public CleverName Name { get; set; } = new();
    public CleverRoles Roles { get; set; } = new();
}

internal class CleverName
{
    public String First { get; set; } = null!;
    public String Last { get; set; } = null!;
}

internal class CleverRoles
{
    public CleverStudent? Student { get; set; }
    public CleverTeacher? Teacher { get; set; }
    public CleverStaff? Staff { get; set; }
    [JsonPropertyName("district_admin")] public CleverDistrictAdmin? DistrictAdmin { get; set; }
}

internal class CleverStudent
{
    [JsonPropertyName("sis_id")] public String? SisId { get; set; }
    public String? School { get; set; }
    public IList<String> Schools { get; set; } = [];
    public String? Grade { get; set; }
}

internal class CleverTeacher
{
    [JsonPropertyName("sis_id")] public String? SisId { get; set; }
    public String? School { get; set; }
    public IList<String> Schools { get; set; } = [];
}

internal class CleverStaff
{
    [JsonPropertyName("staff_id")] public String? SisId { get; set; }
    public IList<String> Schools { get; set; } = [];
}

internal class CleverDistrictAdmin;