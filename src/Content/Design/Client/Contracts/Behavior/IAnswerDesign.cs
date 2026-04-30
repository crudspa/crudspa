namespace Crudspa.Content.Design.Client.Contracts.Behavior;

public interface IAnswerDesign : IDesign
{
    Question Question { get; set; }
    void PrepareForSave();
}