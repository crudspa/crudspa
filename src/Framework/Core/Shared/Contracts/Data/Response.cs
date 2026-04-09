using System.Diagnostics.CodeAnalysis;

namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Response
{
    public Response() { }
    public Response(String errorMessage) => Errors.Add(new() { Message = errorMessage });
    public List<Error> Errors { get; set; } = [];
    public virtual Boolean Ok => Errors.Count == 0;
    public virtual String ErrorMessages => Errors.Aggregate(String.Empty, (current, error) => current + error.Message + Environment.NewLine).Trim();
}

public class Response<T> : Response where T : class?
{
    public Response() { }

    public Response(T? value) => Value = value;

    public Response(String errorMessage) : base(errorMessage) { }

    public T? Value { get; set; }

    [MemberNotNullWhen(true, nameof(Value))] public override Boolean Ok => base.Ok && Value is not null;
}