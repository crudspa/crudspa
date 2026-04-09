namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface IRelates : IUnique
{
    ObservableCollection<RelatedEntity> Relations { get; set; }
}