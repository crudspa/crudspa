namespace Crudspa.Framework.Core.Server.Wrappers;

public class ServiceWrappersCore(ILogger<ServiceWrappersCore> logger) : IServiceWrappers
{
    public async Task<Response> Try(Request request, Func<Response, Task> func)
    {
        var response = new Response();

        try
        {
            await func.Invoke(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
            response.AddError(Constants.ErrorMessages.GenericError);
        }

        return response;
    }

    public async Task<Response<T>> Try<T>(Request request, Func<Response<T>, Task<T>> func)
        where T : class?
    {
        var response = new Response<T>();

        try
        {
            response.Value = await func.Invoke(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
            response.AddError(Constants.ErrorMessages.GenericError);
        }

        return response;
    }

    public async Task<Response<T>> Validate<T, TU>(Request<TU> request, Func<Response<T>, Task<T>> func)
        where T : class?
        where TU : class, IValidates
    {
        var response = new Response<T>();

        try
        {
            response.Errors = request.Value.Validate();

            if (response.Errors.IsEmpty())
                response.Value = await func.Invoke(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
            response.AddError(Constants.ErrorMessages.GenericError);
        }

        return response;
    }

    public async Task<Response> Validate<TU>(Request<TU> request, Func<Response, Task> func)
        where TU : class, IValidates
    {
        var response = new Response();

        try
        {
            response.Errors = request.Value.Validate();

            if (response.Errors.IsEmpty())
                await func.Invoke(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
            response.AddError(Constants.ErrorMessages.GenericError);
        }

        return response;
    }

    public async Task<Response<T>> ValidateAll<T, TU>(Request<IList<TU>> request, Func<Response<T>, Task<T>> func)
        where T : class
        where TU : class, IValidates
    {
        var response = new Response<T>();

        try
        {
            for (var index = 0; index < request.Value.Count; index++)
            {
                var count = index + 1;
                var validates = request.Value[index];
                var errors = validates.Validate();
                errors.Apply(x => x.Message += $" (Record #{count})");
                response.AddErrors(errors);
            }

            if (response.Errors.IsEmpty())
                await func.Invoke(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
            response.AddError(Constants.ErrorMessages.GenericError);
        }

        return response;
    }

    public async Task<Response> ValidateAll<TU>(Request<IList<TU>> request, Func<Response, Task> tryFunc)
        where TU : class, IValidates
    {
        var response = new Response();

        try
        {
            for (var index = 0; index < request.Value.Count; index++)
            {
                var count = index + 1;
                var validates = request.Value[index];
                var errors = validates.Validate();
                errors.Apply(x => x.Message += $" (Record #{count})");
                response.AddErrors(errors);
            }

            if (response.Errors.IsEmpty())
                await tryFunc.Invoke(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
            response.AddError(Constants.ErrorMessages.GenericError);
        }

        return response;
    }
}