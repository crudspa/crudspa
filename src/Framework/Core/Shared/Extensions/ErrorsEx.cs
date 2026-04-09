namespace Crudspa.Framework.Core.Shared.Extensions;

public static class ErrorsEx
{
    public static List<Error> Validate(Action<List<Error>> validationAction)
    {
        var errors = new List<Error>();

        try
        {
            validationAction(errors);
        }
        catch (Exception ex)
        {
            errors.AddError(Constants.ErrorMessages.ValidationException + $" ({ex.Message})");
        }

        return errors;
    }

    public static async Task<List<Error>> Validate(Func<List<Error>, Task> validationAction)
    {
        var errors = new List<Error>();

        try
        {
            await validationAction(errors);
        }
        catch (Exception ex)
        {
            errors.AddError(Constants.ErrorMessages.ValidationException + $" ({ex.Message})");
        }

        return errors;
    }

    extension(IList<Error> errors)
    {
        public String ToStringWithNewlines() =>
            errors.Aggregate(String.Empty, (current, error) => current + error.Message + Environment.NewLine);

        public String ToStringWithSpaces() =>
            errors.Aggregate(String.Empty, (current, error) => current + error.Message + " ").TrimEnd();

        public void PrependMessages(String stringToPrepend) =>
            errors.Apply(x => x.Message = stringToPrepend + x.Message);

        public Error AddError(String? message)
        {
            return errors.AddError(message, null);
        }

        public Error AddError(String? message, String? field)
        {
            var error = new Error
            {
                Message = message,
                Field = field,
            };

            errors.Add(error);

            return error;
        }
    }

    extension(ObservableCollection<Error>? existingErrors)
    {
        public void AddRange(IEnumerable<Error> newErrors)
        {
            if (newErrors.IsEmpty())
                return;

            existingErrors ??= [];

            foreach (var newError in newErrors)
                existingErrors.Add(newError);
        }
    }

    extension(Response response)
    {
        public Error AddError(String? message)
        {
            return response.AddError(message, null);
        }

        public Error AddError(String? message, String? field)
        {
            var error = new Error
            {
                Message = message,
                Field = field,
            };

            response.Errors.Add(error);

            return error;
        }

        public void AddErrors(IEnumerable<Error> newErrors)
        {
            response.Errors.AddRange(newErrors);
        }
    }

    extension(String? propertyValue)
    {
        public void ValidateColorProperty(String? propertyName, IList<Error> errors)
        {
            if (propertyValue.HasSomething())
            {
                if (propertyValue.Length > 10)
                    errors.AddError($"{propertyName!.InsertSpaces()} cannot be longer than 10 characters.", propertyName);
                else if (!propertyValue.IsValidCssColor())
                    errors.AddError($"{propertyName!.InsertSpaces()} must be a valid hex color (a '#' sign followed by 3, 6, or 8 hexadecimal characters, such as '#ff0055').", propertyName);
            }
        }

        public void ValidateUnitProperty(String? propertyName, IList<Error> errors)
        {
            if (propertyValue.HasSomething())
            {
                if (propertyValue.Length > 10)
                    errors.AddError($"{propertyName!.InsertSpaces()} cannot be longer than 10 characters.", propertyName);
                else if (!propertyValue.IsValidCssPositiveNumber())
                    errors.AddError($"{propertyName!.InsertSpaces()} must be a positive number.", propertyName);
            }
        }

        public void ValidateWidthProperty(String? propertyName, IList<Error> errors)
        {
            if (propertyValue.HasSomething())
            {
                if (propertyValue.Length > 10)
                    errors.AddError($"{propertyName!.InsertSpaces()} cannot be longer than 10 characters.", propertyName);
                else if (!propertyValue.IsValidCssWidth())
                    errors.AddError($"{propertyName!.InsertSpaces()} must be a number followed by 'em' or 'px'.", propertyName);
            }
        }

        public void ValidateResponsiveLengthProperty(String? propertyName, IList<Error> errors)
        {
            if (propertyValue.HasSomething())
            {
                if (propertyValue.Length > 10)
                    errors.AddError($"{propertyName!.InsertSpaces()} cannot be longer than 10 characters.", propertyName);
                else if (!propertyValue.IsValidCssResponsiveLength())
                    errors.AddError($"{propertyName!.InsertSpaces()} must be a number followed by a supported CSS unit such as 'px', 'em', 'rem', 'vw', or 'vh'.", propertyName);
            }
        }

        public void ValidateLengthOrPercentageProperty(String? propertyName, IList<Error> errors)
        {
            if (propertyValue.HasSomething())
            {
                if (propertyValue.Length > 10)
                    errors.AddError($"{propertyName!.InsertSpaces()} cannot be longer than 10 characters.", propertyName);
                else if (!propertyValue.IsValidCssLengthOrPercentage())
                    errors.AddError($"{propertyName!.InsertSpaces()} must be a percentage or a number followed by a supported CSS unit such as 'px', 'em', 'rem', 'vw', or 'vh'.", propertyName);
            }
        }

        public void ValidateResponsiveWidthProperty(String? propertyName, IList<Error> errors)
        {
            if (propertyValue.HasSomething())
            {
                if (propertyValue.Length > 10)
                    errors.AddError($"{propertyName!.InsertSpaces()} cannot be longer than 10 characters.", propertyName);
                else if (!propertyValue.IsValidCssResponsiveWidth())
                    errors.AddError($"{propertyName!.InsertSpaces()} must be 'auto', a percentage, or a number followed by a supported CSS unit such as 'px', 'em', 'rem', 'vw', or 'vh'.", propertyName);
            }
        }

        public void ValidateWeightProperty(String? propertyName, IList<Error> errors)
        {
            if (propertyValue.HasSomething())
            {
                if (propertyValue.Length > 10)
                    errors.AddError($"{propertyName!.InsertSpaces()} cannot be longer than 10 characters.", propertyName);
                else if (!propertyValue.IsValidCssWeight())
                    errors.AddError($"{propertyName!.InsertSpaces()} must be 100, 200... to 900.", propertyName);
            }
        }
    }
}