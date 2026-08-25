namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class MorePopulationTokens
{
    public static void AddTo(IList<PopulationToken> tokens, Boolean includeDistrictNames = false)
    {
        Add(tokens, "Title", "Recipient title");
        Add(tokens, "SchoolName", "Represented school names");
        Add(tokens, "DistrictName", "Represented district names");

        if (includeDistrictNames)
            Add(tokens, "DistrictNames", "Represented district names");

        Add(tokens, "MORETeacherLeader", "MORE Teacher Leader names for represented schools");
        Add(tokens, "LessonStartDate", "Lesson start date");
        Add(tokens, "SchoolTeacherLoginPercentage", "Teacher sign-in percentage for represented schools");
        Add(tokens, "SchoolMOREDigital101Percentage", "MORE Digital 101 completion percentage for represented schools");
        Add(tokens, "SchoolStudentSigninPercentage", "Student sign-in percentage for represented schools");
        Add(tokens, "SchoolStudentOneGamePercentage", "One-game completion percentage for represented schools");
        Add(tokens, "SchoolScienceVocabularyPercentage", "Science vocabulary completion percentage for represented schools");
        Add(tokens, "SchoolScienceCCPercentage", "Science comprehension challenge completion percentage for represented schools");
        Add(tokens, "SchoolSSVocabularyPercentage", "Social Studies vocabulary completion percentage for represented schools");
        Add(tokens, "SchoolSSCCPercentage", "Social Studies comprehension challenge completion percentage for represented schools");
        Add(tokens, "SchoolOneTeacherAudioPercentage", "Teacher audio recording percentage for represented schools");
        Add(tokens, "SchoolAllFourTeacherAudioPercentage", "Four-subject teacher audio recording percentage for represented schools");
        Add(tokens, "DistrictTeacherLoginPercentage", "Teacher sign-in percentage for represented districts");
        Add(tokens, "DistrictMOREDigital101Percentage", "MORE Digital 101 completion percentage for represented districts");
        Add(tokens, "DistrictStudentSigninPercentage", "Student sign-in percentage for represented districts");
        Add(tokens, "DistrictStudentOneGamePercentage", "One-game completion percentage for represented districts");
        Add(tokens, "DistrictScienceVocabularyPercentage", "Science vocabulary completion percentage for represented districts");
        Add(tokens, "DistrictScienceCCPercentage", "Science comprehension challenge completion percentage for represented districts");
        Add(tokens, "DistrictSSVocabularyPercentage", "Social Studies vocabulary completion percentage for represented districts");
        Add(tokens, "DistrictSSCCPercentage", "Social Studies comprehension challenge completion percentage for represented districts");
        Add(tokens, "DistrictOneTeacherAudioPercentage", "Teacher audio recording percentage for represented districts");
        Add(tokens, "DistrictAllFourTeacherAudioPercentage", "Four-subject teacher audio recording percentage for represented districts");
    }

    private static void Add(IList<PopulationToken> tokens, String key, String description) =>
        tokens.Add(new() { Key = key, Description = description, Ordinal = tokens.Count });
}