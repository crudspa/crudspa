namespace Crudspa.Framework.Core.Shared.Extensions;

public static class PluralizationEx
{
    private static Boolean IsVowel(Char c) => "aeiouAEIOU".IndexOf(c) >= 0;

    extension(String? input)
    {
        public String PluralizeNaively()
        {
            if (input.HasNothing())
                return String.Empty;

            if (Irregulars.TryGetValue(input, out var irregular))
                return irregular;

            if (input.EndsWith("y", StringComparison.InvariantCultureIgnoreCase) && input.Length > 1 && !IsVowel(input[^2]))
                return input.Substring(0, input.Length - 1) + "ies";

            if (input.EndsWith("us", StringComparison.InvariantCultureIgnoreCase))
                return input.Substring(0, input.Length - 2) + "i";

            if (input.EndsWith("is", StringComparison.InvariantCultureIgnoreCase))
                return input.Substring(0, input.Length - 2) + "es";

            if (input.EndsWith("ox", StringComparison.InvariantCultureIgnoreCase))
                return input + "es";

            if (input.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) || input.EndsWith("sh", StringComparison.InvariantCultureIgnoreCase) || input.EndsWith("ch", StringComparison.InvariantCultureIgnoreCase) || input.EndsWith("x", StringComparison.InvariantCultureIgnoreCase) || input.EndsWith("z", StringComparison.InvariantCultureIgnoreCase))
                return input + "es";

            return input + "s";
        }
    }

    public static readonly Dictionary<String, String> Irregulars = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "alumna", "alumnae" },
        { "alumnus", "alumni" },
        { "analysis", "analyses" },
        { "appendix", "appendices" },
        { "axis", "axes" },
        { "bacterium", "bacteria" },
        { "basis", "bases" },
        { "cactus", "cacti" },
        { "child", "children" },
        { "corpus", "corpora" },
        { "crisis", "crises" },
        { "criterion", "criteria" },
        { "curriculum", "curricula" },
        { "datum", "data" },
        { "diagnosis", "diagnoses" },
        { "die", "dice" },
        { "echo", "echoes" },
        { "ellipsis", "ellipses" },
        { "focus", "foci" },
        { "foot", "feet" },
        { "fungus", "fungi" },
        { "genus", "genera" },
        { "goose", "geese" },
        { "half", "halves" },
        { "hero", "heroes" },
        { "hypothesis", "hypotheses" },
        { "index", "indexes" },
        { "knife", "knives" },
        { "leaf", "leaves" },
        { "life", "lives" },
        { "louse", "lice" },
        { "man", "men" },
        { "matrix", "matrices" },
        { "medium", "media" },
        { "mouse", "mice" },
        { "nucleus", "nuclei" },
        { "oasis", "oases" },
        { "octopus", "octopuses" },
        { "opus", "opera" },
        { "ox", "oxen" },
        { "paralysis", "paralyses" },
        { "parenthesis", "parentheses" },
        { "person", "people" },
        { "phenomenon", "phenomena" },
        { "potato", "potatoes" },
        { "quiz", "quizzes" },
        { "shelf", "shelves" },
        { "spectrum", "spectra" },
        { "stimulus", "stimuli" },
        { "syllabus", "syllabi" },
        { "synopsis", "synopses" },
        { "thesis", "theses" },
        { "tomato", "tomatoes" },
        { "tooth", "teeth" },
        { "torpedo", "torpedoes" },
        { "vertex", "vertices" },
        { "veto", "vetoes" },
        { "virus", "viruses" },
        { "wife", "wives" },
        { "wolf", "wolves" },
        { "woman", "women" },
    };
}