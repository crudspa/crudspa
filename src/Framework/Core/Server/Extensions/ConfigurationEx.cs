namespace Crudspa.Framework.Core.Server.Extensions;

public static class ConfigurationEx
{
    extension(IConfiguration configuration)
    {
        public String ReadString(String key, Boolean throwIfEmpty = true)
        {
            var value = configuration[key];

            if (throwIfEmpty && value.HasNothing())
                throw new($"Configuration key '{key}' is missing or empty.");

            return value ?? String.Empty;
        }

        public Boolean ReadBoolean(String key, Boolean throwIfEmpty = true)
        {
            var value = configuration[key];

            if (throwIfEmpty && value.HasNothing())
                throw new($"Configuration key '{key}' is missing or empty.");

            try
            {
                return Boolean.Parse(value ?? "false");
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Configuration key '{key}' contains an invalid boolean value.", ex);
            }
        }

        public Guid ReadGuid(String key, Boolean throwIfEmpty = true)
        {
            var value = configuration[key];

            if (throwIfEmpty && value.HasNothing())
                throw new($"Configuration key '{key}' is missing or empty.");
            try
            {
                return Guid.Parse(value ?? Guid.Empty.ToString());
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Configuration key '{key}' contains an invalid guid value.", ex);
            }
        }

        public Int32 ReadInt(String key, Boolean throwIfEmpty = true)
        {
            var value = configuration[key];

            if (throwIfEmpty && value.HasNothing())
                throw new($"Configuration key '{key}' is missing or empty.");
            try
            {
                return Int32.Parse(value ?? "0");
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Configuration key '{key}' contains an invalid integer value.", ex);
            }
        }
    }
}