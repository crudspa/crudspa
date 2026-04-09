namespace Crudspa.Education.Student.Server.Sproxies;

public static class GameActivitySelectForBatch
{
    public static async Task<IList<GameActivity>> Execute(String connection, Guid? assignmentBatchId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.GameActivitySelectForBatch";

        command.AddParameter("@AssignmentBatchId", assignmentBatchId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var gameActivities = new List<GameActivity>();

            while (await reader.ReadAsync())
                gameActivities.Add(ReadGameActivity(reader));

            var activityChoices = new List<ActivityChoice>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                activityChoices.Add(PageRunSelectCommon.ReadActivityChoice(reader));

            var choicesByActivityId = activityChoices.ToLookup(x => x.ActivityId);

            foreach (var gameActivity in gameActivities)
            {
                var choicesForActivity = choicesByActivityId[gameActivity.Activity!.Id].ToList();

                gameActivity.Activity!.ActivityChoices = gameActivity.Activity.ActivityTypeShuffleChoices == true
                    ? choicesForActivity.Shuffle().ToObservable()
                    : choicesForActivity.OrderBy(x => x.Ordinal).ToObservable();
            }

            return gameActivities;
        });
    }

    public static GameActivity ReadGameActivity(SqlDataReader reader)
    {
        var gameActivity = GameSectionSelectForGame.ReadGameActivity(reader);

        gameActivity.AssignmentBatchId = reader.ReadGuid(45);
        gameActivity.Ordinal = reader.ReadInt32(46);

        gameActivity.Activity!.Assignment = new()
        {
            Id = reader.ReadGuid(47),
            AssignmentBatchId = gameActivity.AssignmentBatchId,
            Assigned = reader.ReadDateTimeOffset(48),
            Started = reader.ReadDateTimeOffset(49),
            Finished = reader.ReadDateTimeOffset(50),
            StatusId = reader.ReadGuid(51),
            StatusName = reader.ReadString(52),
        };

        return gameActivity;
    }
}