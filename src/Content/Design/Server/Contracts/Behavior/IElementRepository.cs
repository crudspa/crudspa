namespace Crudspa.Content.Design.Server.Contracts.Behavior;

public interface IElementRepository
{
    Task<SectionElement> Create(ElementType elementType, Guid? sectionId, Int32? ordinal);
    Task<IList<Error>> Validate(String connection, SectionElement element);
    Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element);
    Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element);
    Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element);
}