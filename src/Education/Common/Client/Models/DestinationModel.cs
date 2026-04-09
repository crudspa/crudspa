namespace Crudspa.Education.Common.Client.Models;

public class DestinationModel
{
    public enum States
    {
        Empty, Filled, Valid, Invalid,
    }

    public States State { get; set; } = States.Empty;
    public PieceModel? TargetPiece { get; set; }
    public PieceModel? PlacedPiece { get; set; }
    public Guid? ModelId { get; set; }
    public List<Guid>? TargetIds { get; set; }

    public DestinationModel(ActivityChoice? choice, List<Guid>? targetIds)
    {
        if (choice is not null)
            TargetPiece = new(choice);

        else if (targetIds is not null)
        {
            ModelId = Guid.NewGuid();
            TargetIds = targetIds;
        }
    }

    public String CssClass
    {
        get
        {
            return State switch
            {
                States.Empty => "empty",
                States.Filled => "default",
                States.Valid => "valid",
                States.Invalid => "invalid",
                _ => "empty",
            };
        }
    }
}