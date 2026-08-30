using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

internal static class RosterRunReader
{
    public static RosterRun Read(SqlDataReader reader) => new()
    {
        Id = reader.ReadGuid(0)!.Value,
        RosterSourceId = reader.ReadGuid(1)!.Value,
        Kind = reader.ReadString(2)!,
        Status = reader.ReadString(3)!,
        Started = reader.ReadDateTimeOffset(4)!.Value,
        Completed = reader.ReadDateTimeOffset(5),
        Checkpoint = reader.ReadString(6),
        SchoolCount = reader.ReadInt32(7)!.Value,
        TermCount = reader.ReadInt32(8)!.Value,
        CourseCount = reader.ReadInt32(9)!.Value,
        UserCount = reader.ReadInt32(10)!.Value,
        RoleCount = reader.ReadInt32(11)!.Value,
        ClassCount = reader.ReadInt32(12)!.Value,
        EnrollmentCount = reader.ReadInt32(13)!.Value,
        AddCount = reader.ReadInt32(14)!.Value,
        UpdateCount = reader.ReadInt32(15)!.Value,
        RemoveCount = reader.ReadInt32(16)!.Value,
        IssueCount = reader.ReadInt32(17)!.Value,
    };
}