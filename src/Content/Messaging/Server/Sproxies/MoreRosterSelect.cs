namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class MoreRosterSelect
{
    public static IList<PopulationToken> CreateTokens()
    {
        List<PopulationToken> tokens =
        [
            new() { Key = "FirstName", Description = "Recipient first name", Ordinal = 0 },
            new() { Key = "LastName", Description = "Recipient last name", Ordinal = 1 },
            new() { Key = "Email", Description = "Recipient email address", Ordinal = 2 },
            new() { Key = "SchoolNames", Description = "Represented school names", Ordinal = 3 },
            new() { Key = "DistrictNames", Description = "Represented district names", Ordinal = 4 },
            new() { Key = "RoleNames", Description = "Recipient roles", Ordinal = 5 },
        ];

        MorePopulationTokens.AddTo(tokens);
        return tokens;
    }

    public static async Task<PopulationResult> Execute(
        String connection, Guid? sessionId, Guid? portalId, Guid organizationId, String? populationKey, Guid? activationScopeId)
    {
        await using var command = new SqlCommand("ContentMessaging.MoreRosterSelect");
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);
        command.AddParameter("@OrganizationId", organizationId);
        command.AddParameter("@PopulationKey", 75, populationKey);
        command.AddParameter("@ActivationScopeId", activationScopeId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var result = new PopulationResult
            {
                Tokens = CreateTokens(),
            };

            while (await reader.ReadAsync())
            {
                var contactId = reader.ReadGuid(0)!.Value;
                result.Members.Add(new() { ContactId = contactId });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "FirstName", Value = reader.ReadString(1) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "LastName", Value = reader.ReadString(2) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "Email", Value = reader.ReadString(3) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "SchoolNames", Value = reader.ReadString(4) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "DistrictNames", Value = reader.ReadString(5) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "RoleNames", Value = reader.ReadString(6) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "Title", Value = reader.ReadString(7) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "SchoolName", Value = reader.ReadString(8) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "DistrictName", Value = reader.ReadString(9) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "MORETeacherLeader", Value = reader.ReadString(10) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "LessonStartDate", Value = reader.ReadString(11) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "SchoolTeacherLoginPercentage", Value = reader.ReadString(12) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "SchoolMOREDigital101Percentage", Value = reader.ReadString(13) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "SchoolStudentSigninPercentage", Value = reader.ReadString(14) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "SchoolStudentOneGamePercentage", Value = reader.ReadString(15) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "SchoolScienceVocabularyPercentage", Value = reader.ReadString(16) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "SchoolScienceCCPercentage", Value = reader.ReadString(17) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "SchoolSSVocabularyPercentage", Value = reader.ReadString(18) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "SchoolSSCCPercentage", Value = reader.ReadString(19) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "SchoolOneTeacherAudioPercentage", Value = reader.ReadString(20) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "SchoolAllFourTeacherAudioPercentage", Value = reader.ReadString(21) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "DistrictTeacherLoginPercentage", Value = reader.ReadString(22) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "DistrictMOREDigital101Percentage", Value = reader.ReadString(23) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "DistrictStudentSigninPercentage", Value = reader.ReadString(24) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "DistrictStudentOneGamePercentage", Value = reader.ReadString(25) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "DistrictScienceVocabularyPercentage", Value = reader.ReadString(26) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "DistrictScienceCCPercentage", Value = reader.ReadString(27) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "DistrictSSVocabularyPercentage", Value = reader.ReadString(28) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "DistrictSSCCPercentage", Value = reader.ReadString(29) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "DistrictOneTeacherAudioPercentage", Value = reader.ReadString(30) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "DistrictAllFourTeacherAudioPercentage", Value = reader.ReadString(31) });
            }

            return result;
        });
    }
}