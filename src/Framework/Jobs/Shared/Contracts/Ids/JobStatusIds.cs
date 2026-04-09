namespace Crudspa.Framework.Jobs.Shared.Contracts.Ids;

public class JobStatusIds
{
    public static readonly Guid Pending = new("5e2d54a0-5774-4cae-8391-0b6ac31d4f60");
    public static readonly Guid Running = new("28886325-475c-4d3e-9624-96e9c151775d");
    public static readonly Guid Completed = new("81c1ccdb-cbf3-4a6a-845e-ca8839c17d2d");
    public static readonly Guid Failed = new("c6416f41-8dc5-424d-a53b-04f13ad3568d");
    public static readonly Guid Canceled = new("3461ccbd-4ceb-4de4-94e5-0c6b3e36ae9d");
}