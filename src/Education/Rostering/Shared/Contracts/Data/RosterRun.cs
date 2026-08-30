namespace Crudspa.Education.Rostering.Shared.Contracts.Data;

public class RosterRun
{
    public Guid Id { get; set; }
    public Guid RosterSourceId { get; set; }
    public String Kind { get; set; } = null!;
    public String Status { get; set; } = null!;
    public DateTimeOffset Started { get; set; }
    public DateTimeOffset? Completed { get; set; }
    public String? Checkpoint { get; set; }
    public Int32 SchoolCount { get; set; }
    public Int32 TermCount { get; set; }
    public Int32 CourseCount { get; set; }
    public Int32 UserCount { get; set; }
    public Int32 RoleCount { get; set; }
    public Int32 ClassCount { get; set; }
    public Int32 EnrollmentCount { get; set; }
    public Int32 AddCount { get; set; }
    public Int32 UpdateCount { get; set; }
    public Int32 RemoveCount { get; set; }
    public Int32 IssueCount { get; set; }
}