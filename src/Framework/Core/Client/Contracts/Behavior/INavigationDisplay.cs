namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface INavigationDisplay
{
    Portal Portal { get; set; }
    RenderFragment? Branding { get; set; }
    RenderFragment? Footer { get; set; }
    RenderFragment? AuthViews { get; set; }
    String? Styles { get; set; }
}