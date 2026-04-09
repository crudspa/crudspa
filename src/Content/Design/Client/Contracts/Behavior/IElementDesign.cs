using IDesign = Crudspa.Framework.Core.Client.Contracts.Behavior.IDesign;

namespace Crudspa.Content.Design.Client.Contracts.Behavior;

public interface IElementDesign : IDesign
{
    SectionElement Element { get; set; }
    void PrepareForSave();
}