using Crudspa.Content.Design.Server.Services;
using Crudspa.Content.Design.Shared.Contracts.Data;
using Crudspa.Content.Display.Server.Services;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Crudspa.Framework.Core.Server.Contracts.Data;
using Crudspa.Framework.Core.Server.Wrappers;
using Crudspa.Framework.Core.Shared.Contracts.Data;
using Crudspa.Framework.Core.Shared.Contracts.Ids;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using Publication = Crudspa.Content.Design.Shared.Contracts.Data.Publication;

// Integration checks against the disposable shared sample database. All fixture rows are removed in finally.
const string connectionString = "Server=localhost;Database=Crudspa-Local;Integrated Security=True;TrustServerCertificate=True";
var config = new TestConfig(connectionString);
var wrappers = new ServiceWrappersCore(NullLogger<ServiceWrappersCore>.Instance);
var author = new PublicationServiceSql(wrappers, new SqlWrappersCore(config), config, null!);
var groups = new PublicationGroupServiceSql(wrappers, new SqlWrappersCore(config), config);
var display = new PublicationRunServiceSql(wrappers, config);
var session = Guid.NewGuid();
var foreignSession = Guid.NewGuid();
var publicSession = Guid.NewGuid();
var otherPublicSession = Guid.NewGuid();
var consumer = Guid.Parse("73410fd3-3681-46d3-800e-a08670e291cf");
var composer = Guid.Parse("aea2c861-459a-490c-b7c3-30e5156fec9f");
var catalog = Guid.Parse("651a367c-a7dd-4fe8-be5a-b70ef275a8ec");
var paperType = Guid.Parse("ade43b28-8c99-5900-9230-8734e8d75706");
int checks = 0;
void Check(bool value, string description)
{
    if (!value) throw new InvalidOperationException(description);
    Console.WriteLine("PASS " + description);
    checks++;
}
async Task<object?> Scalar(string sql)
{
    await using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();
    await using var command = new SqlCommand(sql, connection);
    return await command.ExecuteScalarAsync();
}
async Task<Publication> Fetch(Guid? id)
{
    var response = await author.Fetch(new(session, new() { Id = id }));
    Check(response.Ok, "Author can reload the complete publication batch");
    return response.Value!;
}
try
{
    await Scalar($"""
        insert Framework.Session(Id,Started,PortalId,UserId,UserAdded)
        values('{session}',sysdatetimeoffset(),'{composer}',(select top 1 u.Id from Framework.[User-Active] u join Framework.[Portal-Active] p on p.OwnerId=u.OrganizationId where p.Id='{composer}'),sysdatetimeoffset()),
        ('{foreignSession}',sysdatetimeoffset(),'{catalog}',(select top 1 u.Id from Framework.[User-Active] u join Framework.[Portal-Active] p on p.OwnerId=u.OrganizationId where p.Id='{catalog}'),sysdatetimeoffset()),
        ('{publicSession}',sysdatetimeoffset(),'{consumer}',null,null),
        ('{otherPublicSession}',sysdatetimeoffset(),'{composer}',null,null);
        """);
    var group = await groups.Add(new(session, new() { PortalId = consumer, Name = "Publication integration fixture" }));
    var otherGroup = await groups.Add(new(session, new() { PortalId = composer, Name = "Other site integration fixture" }));
    Check(group.Ok && otherGroup.Ok, "Groups can be authored separately for two sites");
    var publication = new Publication
    {
        PortalId = consumer, GroupId = group.Value!.Id, StatusId = ContentStatusIds.Draft,
        Title = new string('A', 151), Citation = "Integration fixture citation.", Year = 2024,
        PublicationResources = [new() { Id = Guid.NewGuid(), TypeId = paperType, Url = "https://example.org/paper", Ordinal = 0 }]
    };
    var added = await author.Add(new(session, publication));
    Check(added.Ok, "A title longer than 150 characters saves with its resource batch");
    publication = await Fetch(added.Value!.Id);
    Check(publication.PublicationResources.Count == 1, "The parent and ordered resource persist together");
    var published = await display.FetchAll(new(publicSession));
    Check(published.Ok && published.Value!.All(x => x.Id != publication.Id), "Drafts stay out of public results");
    publication.StatusId = ContentStatusIds.Complete;
    publication.Citation = " ";
    Check(!(await author.Save(new(session, publication))).Ok, "Publishing requires a citation");
    publication.Citation = "Integration fixture citation.";
    Check((await author.Save(new(session, publication))).Ok, "A Complete publication can be saved");
    published = await display.FetchAll(new(publicSession));
    Check(published.Ok && published.Value!.Single(x => x.Id == publication.Id).PublicationResources.Count == 1,
        "Public results contain Complete publications and their resource batch");
    var otherPublic = await display.FetchAll(new(otherPublicSession));
    Check(otherPublic.Ok && otherPublic.Value!.All(x => x.Id != publication.Id), "Public results are isolated to the session site");
    var denied = await author.Fetch(new(foreignSession, new() { Id = publication.Id }));
    Check(denied.Value is null, "A foreign organization cannot read the parent or its resources");
    var deniedCatalog = await author.FetchForPortal(new(foreignSession, new() { Id = consumer }));
    Check(deniedCatalog.Ok && deniedCatalog.Value!.Count == 0, "A foreign organization cannot list another site's authoring catalog");
    await using (var connection = new SqlConnection(connectionString))
    {
        await connection.OpenAsync();
        await using var command = new SqlCommand("ContentDesign.PublicationSelectForPortal", connection)
        { CommandType = System.Data.CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@SessionId", foreignSession);
        command.Parameters.AddWithValue("@PortalId", consumer);
        await using var reader = await command.ExecuteReaderAsync();
        Check(!await reader.ReadAsync() && await reader.NextResultAsync() && !await reader.ReadAsync(),
            "Both SQL result sets enforce authoring scope, including resource rows");
    }
    Check(!(await author.Save(new(foreignSession, publication))).Ok, "A foreign organization cannot edit a publication");
    publication.GroupId = otherGroup.Value!.Id;
    Check(!(await author.Save(new(session, publication))).Ok, "A group from another site is rejected even within the same organization");
    publication.GroupId = group.Value.Id;
    var oldTitle = publication.Title;
    publication.Title = "This change must roll back";
    publication.PublicationResources[0].TypeId = Guid.NewGuid();
    Check(!(await author.Save(new(session, publication))).Ok, "An invalid child causes save failure");
    publication = await Fetch(publication.Id);
    Check(publication.Title == oldTitle && publication.PublicationResources[0].TypeId == paperType,
        "A failed batch rolls back both parent and children");
    publication.PublicationResources[0].Url = "javascript:alert(1)";
    Check(!(await author.Save(new(session, publication))).Ok, "Non-HTTP resource destinations are rejected");
    publication.PublicationResources[0].Url = null;
    Check(!(await author.Save(new(session, publication))).Ok, "A resource must have exactly one destination");
    publication.PublicationResources[0].Url = "https://example.org/paper";
    publication.PublicationResources.Add(new() { Id = Guid.NewGuid(), TypeId = paperType, Url = "https://example.org/supplement", Ordinal = 0 });
    Check((await author.Save(new(session, publication))).Ok, "Adding a resource to an existing publication succeeds");
    publication = await Fetch(publication.Id);
    var first = publication.PublicationResources[0];
    publication.PublicationResources.RemoveAt(0);
    publication.PublicationResources.Add(first);
    for (var index = 0; index < publication.PublicationResources.Count; index++)
        publication.PublicationResources[index].Ordinal = index;
    Check((await author.Save(new(session, publication))).Ok, "Resource ordering saves as part of the batch");
    var ordered = await Fetch(publication.Id);
    Check(ordered.PublicationResources.Last().Id == first.Id, "Resource order survives reload");
    var extra = await author.Add(new(session, new() { PortalId = composer, StatusId = ContentStatusIds.Draft, Title = "Other site fixture" }));
    Check(extra.Ok, "A second site can author its own publication");
    Check(!(await author.SaveOrder(new(session, new List<Publication> { publication, extra.Value! }))).Ok,
        "An order operation cannot mix two sites");
    var foreignOrders = await author.SaveOrder(new(foreignSession, new List<Publication> { publication }));
    Check(!foreignOrders.Ok, "Ordering enforces organization ownership");
    Check((await groups.Remove(new(session, group.Value))).Ok, "Deleting a group safely ungroups its publications");
    publication = await Fetch(publication.Id);
    Check(publication.GroupId is null, "A publication remains available after its group is removed");
    publication.StatusId = ContentStatusIds.Retired;
    Check((await author.Save(new(session, publication))).Ok, "A publication can be retired");
    published = await display.FetchAll(new(publicSession));
    Check(published.Ok && published.Value!.All(x => x.Id != publication.Id), "Retired publications disappear from public results");
    Check((await author.Remove(new(session, publication))).Ok, "Removing a publication removes its active resource relationships");
    Check((int)(await Scalar($"select count(*) from Content.[PublicationResource-Active] where PublicationId='{publication.Id}'"))! == 0,
        "No active resources remain under a removed publication");
    Console.WriteLine($"{checks} integration checks passed.");
}
finally
{
    // Only rows authored by the new fixture sessions are removed, including their version history.
    await Scalar($"""
        delete Content.PublicationResource where UpdatedBy in ('{session}','{foreignSession}');
        delete Content.Publication where UpdatedBy in ('{session}','{foreignSession}');
        delete Content.PublicationGroup where UpdatedBy in ('{session}','{foreignSession}');
        delete Framework.Session where Id in ('{session}','{foreignSession}','{publicSession}','{otherPublicSession}');
        """);
}

sealed class TestConfig(string connectionString) : IServerConfigService
{
    public ServerConfig Fetch() => new() { Database = connectionString };
    public void Invalidate() { }
}