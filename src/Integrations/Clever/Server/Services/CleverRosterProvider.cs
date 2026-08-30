using Crudspa.Education.Rostering.Shared.Contracts.Behavior;
using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Education.Rostering.Shared.Contracts.Ids;
using Crudspa.Integrations.Clever.Server.Contracts.Behavior;
using Crudspa.Integrations.Clever.Server.Contracts.Data;

namespace Crudspa.Integrations.Clever.Server.Services;

public class CleverRosterProvider(CleverClient client, ICleverTokenSource tokens) : IRosterProvider
{
    public String Key => RosterProviders.Clever;

    public async Task Stage(
        RosterContext context,
        IRosterSink sink,
        CancellationToken cancellationToken = default)
    {
        var districtId = context.Source.Tenant
            ?? throw new InvalidOperationException("Clever District ID is required.");
        var token = await tokens.Fetch(districtId, cancellationToken);

        await foreach (var values in client.Fetch<CleverSchool>("/v3.0/schools", token, cancellationToken))
            await sink.Write(new()
            {
                Schools = values.Select(x => new RosterSchool(
                    x.Id,
                    x.SisId,
                    x.Name,
                    x.Name.Contains("District Office (Auto-generated)", StringComparison.OrdinalIgnoreCase) ? "district-office" : "school",
                    "active")).ToList(),
            }, cancellationToken);

        await foreach (var values in client.Fetch<CleverTerm>("/v3.0/terms", token, cancellationToken))
            await sink.Write(new()
            {
                Terms = values.Select(x => new RosterTerm(
                    x.Id,
                    null,
                    x.Name ?? x.Id,
                    "term",
                    x.Starts,
                    x.Ends,
                    "active")).ToList(),
            }, cancellationToken);

        await foreach (var values in client.Fetch<CleverCourse>("/v3.0/courses", token, cancellationToken))
            await sink.Write(new()
            {
                Courses = values.Select(x => new RosterCourse(
                    x.Id,
                    null,
                    x.Name ?? x.Number ?? x.Id,
                    x.Number,
                    "active")).ToList(),
            }, cancellationToken);

        await foreach (var values in client.Fetch<CleverUser>("/v3.0/users", token, cancellationToken))
            await sink.Write(Users(values), cancellationToken);

        await foreach (var values in client.Fetch<CleverSection>("/v3.0/sections", token, cancellationToken))
            await sink.Write(Sections(values), cancellationToken);
    }

    private static RosterBatch Users(IEnumerable<CleverUser> values)
    {
        var batch = new RosterBatch();

        foreach (var user in values)
        {
            var sisId = user.Roles.Student?.SisId ?? user.Roles.Teacher?.SisId ?? user.Roles.Staff?.SisId;
            batch.People.Add(new(user.Id, sisId, user.Name.First, user.Name.Last, user.Email, "active", "https://clever.com", user.Id));

            AddRole(batch, user.Id, "student", user.Roles.Student?.Schools, user.Roles.Student?.School, user.Roles.Student?.Grade);
            AddRole(batch, user.Id, "teacher", user.Roles.Teacher?.Schools, user.Roles.Teacher?.School);
            AddRole(batch, user.Id, "staff", user.Roles.Staff?.Schools);

            if (user.Roles.DistrictAdmin is not null)
                batch.Roles.Add(new($"{user.Id}:district-admin:district", user.Id, null, "district-admin", null, true));
        }

        return batch;
    }

    private static void AddRole(
        RosterBatch batch,
        String personId,
        String role,
        IEnumerable<String>? schools,
        String? primarySchool = null,
        String? grade = null)
    {
        foreach (var school in (schools ?? []).Append(primarySchool).OfType<String>().Distinct())
            batch.Roles.Add(new($"{personId}:{role}:{school}", personId, school, role, grade, school == primarySchool));
    }

    private static RosterBatch Sections(IEnumerable<CleverSection> values)
    {
        var batch = new RosterBatch();

        foreach (var section in values)
        {
            batch.Classes.Add(new(
                section.Id,
                section.SisId,
                section.School,
                section.Course,
                section.TermId,
                section.Name,
                section.Grade,
                section.Subject,
                "active"));

            foreach (var student in section.Students ?? [])
                batch.Enrollments.Add(new($"{section.Id}:{student}:student", student, section.Id, section.School, "student", false, "active"));

            foreach (var teacher in (section.Teachers ?? []).Append(section.Teacher).OfType<String>().Distinct())
                batch.Enrollments.Add(new($"{section.Id}:{teacher}:teacher", teacher, section.Id, section.School, "teacher", teacher == section.Teacher, "active"));
        }

        return batch;
    }
}