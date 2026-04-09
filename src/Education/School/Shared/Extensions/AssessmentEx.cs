using Crudspa.Education.School.Shared.Contracts.Ids;

namespace Crudspa.Education.School.Shared.Extensions;

public static class AssessmentEx
{
    private const String LowFriendlyName = "Low Reading Ability";
    private const String MidFriendlyName = "Medium Reading Ability";
    private const String HighFriendlyName = "High Reading Ability";

    extension(Guid? assessmentLevelGroupId)
    {
        public String ToFriendlyName()
        {
            if (!assessmentLevelGroupId.HasValue)
                return MidFriendlyName;

            switch (assessmentLevelGroupId.Value)
            {
                case var x when x == AssessmentLevelIds.Low:
                    return LowFriendlyName;
                case var x when x == AssessmentLevelIds.High:
                    return HighFriendlyName;
                default:
                    return MidFriendlyName;
            }
        }
    }

    extension(String? friendlyName)
    {
        public Guid? FromFriendlyName()
        {
            switch (friendlyName)
            {
                case LowFriendlyName:
                    return AssessmentLevelIds.Low;
                case HighFriendlyName:
                    return AssessmentLevelIds.High;
                default:
                    return AssessmentLevelIds.Mid;
            }
        }
    }

    extension(IList<Selectable> input)
    {
        public List<Selectable> FromFriendlyNames()
        {
            return input.Select(friendly => new Selectable
            {
                Id = friendly.Name.FromFriendlyName(),
                Selected = friendly.Selected,
            }).ToList();
        }
    }
}