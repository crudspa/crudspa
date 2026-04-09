namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface ICardModel<T> : IObservable
    where T : IObservable, INamed
{
    T Entity { get; set; }
    ModalModel ConfirmationModel { get; }
    Boolean Hidden { get; set; }
}