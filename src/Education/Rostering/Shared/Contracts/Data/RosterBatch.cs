namespace Crudspa.Education.Rostering.Shared.Contracts.Data;

public class RosterBatch
{
    public IList<RosterSchool> Schools { get; set; } = [];
    public IList<RosterTerm> Terms { get; set; } = [];
    public IList<RosterCourse> Courses { get; set; } = [];
    public IList<RosterClass> Classes { get; set; } = [];
    public IList<RosterPerson> People { get; set; } = [];
    public IList<RosterRole> Roles { get; set; } = [];
    public IList<RosterEnrollment> Enrollments { get; set; } = [];
    public String? Checkpoint { get; set; }
}

public record RosterSchool(String ExternalId, String? SisId, String Name, String Kind, String Status);

public record RosterTerm(
    String ExternalId,
    String? SisId,
    String Name,
    String Kind,
    DateOnly? Starts,
    DateOnly? Ends,
    String Status);

public record RosterCourse(String ExternalId, String? SisId, String Name, String? Number, String Status);

public record RosterClass(
    String ExternalId,
    String? SisId,
    String SchoolExternalId,
    String? CourseExternalId,
    String? TermExternalId,
    String Name,
    String? Grade,
    String? Subject,
    String Status,
    Boolean? SmallClassroom = null);

public record RosterPerson(
    String ExternalId,
    String? SisId,
    String FirstName,
    String LastName,
    String? Email,
    String Status,
    String? AuthIssuer = null,
    String? AuthSubject = null,
    String? AssessmentLevel = null);

public record RosterRole(
    String ExternalId,
    String PersonExternalId,
    String? SchoolExternalId,
    String Role,
    String? Grade,
    Boolean Primary);

public record RosterEnrollment(
    String ExternalId,
    String PersonExternalId,
    String ClassExternalId,
    String? SchoolExternalId,
    String Role,
    Boolean Primary,
    String Status);