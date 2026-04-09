namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Unit unit)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Title", 75, unit.Title);
        command.AddParameter("@StatusId", unit.StatusId);
        command.AddParameter("@GradeId", unit.GradeId);
        command.AddParameter("@ParentId", unit.ParentId);
        command.AddParameter("@RequiresAchievementId", unit.RequiresAchievementId);
        command.AddParameter("@GeneratesAchievementId", unit.GeneratesAchievementId);
        command.AddParameter("@ImageId", unit.ImageFile.Id);
        command.AddParameter("@GuideText", unit.GuideText);
        command.AddParameter("@GuideAudioId", unit.GuideAudioFile.Id);
        command.AddParameter("@IntroAudioId", unit.IntroAudioFile.Id);
        command.AddParameter("@SongAudioId", unit.SongAudioFile.Id);
        command.AddParameter("@Treatment", unit.Treatment ?? true);
        command.AddParameter("@Control", unit.Control ?? true);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}