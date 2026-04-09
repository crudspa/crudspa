namespace Crudspa.Education.Common.Client.Models;

public class PieceModel(ActivityChoice choice)
{
    public enum States { Default, Selected }

    public Guid? Id { get; set; } = choice.Id;
    public String? Text { get; set; } = choice.Text;
    public States State { get; set; } = States.Default;
}