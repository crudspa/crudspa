namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IServiceWrappers
{
    Task<Response> Try(Request request, Func<Response, Task> func);
    Task<Response<T>> Try<T>(Request request, Func<Response<T>, Task<T>> func) where T : class?;
    Task<Response<T>> Validate<T, TU>(Request<TU> request, Func<Response<T>, Task<T>> func) where T : class? where TU : class?, IValidates;
    Task<Response> Validate<TU>(Request<TU> request, Func<Response, Task> func) where TU : class?, IValidates;
    Task<Response<T>> ValidateAll<T, TU>(Request<IList<TU>> request, Func<Response<T>, Task<T>> func) where T : class where TU : class, IValidates;
    Task<Response> ValidateAll<TU>(Request<IList<TU>> request, Func<Response, Task> tryFunc) where TU : class, IValidates;
}