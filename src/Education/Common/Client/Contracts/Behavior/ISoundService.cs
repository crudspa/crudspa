namespace Crudspa.Education.Common.Client.Contracts.Behavior;

public interface ISoundService
{
    void AchievementGrow();
    void AchievementShrink();
    void ButtonPress();
    void ChoiceCorrect();
    void ChoiceIncorrect();
    void ChoiceSelected();
    void Tada();
}