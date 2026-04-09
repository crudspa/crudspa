namespace Crudspa.Framework.Core.Shared.Extensions;

public static class BoolEx
{
    extension(Boolean value)
    {
        public String ToYesNo()
        {
            return value ? "Yes" : "No";
        }
    }

    extension(Boolean? value)
    {
        public String ToYesNo()
        {
            return value.GetValueOrDefault() ? "Yes" : "No";
        }
    }

    extension(String value)
    {
        public Boolean IsTrue()
        {
            if (!value.HasSomething())
                return false;

            return value.IsBasically("True")
                || value.IsBasically("T")
                || value.IsBasically("Yes")
                || value.IsBasically("Y")
                || value.IsBasically("1");
        }
    }
}