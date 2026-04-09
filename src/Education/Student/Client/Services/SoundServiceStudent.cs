namespace Crudspa.Education.Student.Client.Services;

public class SoundServiceStudent(IJsBridge jsBridge) : ISoundService
{
    public void AchievementGrow() => jsBridge.PlaySound("achievement-grow");
    public void AchievementShrink() => jsBridge.PlaySound("achievement-shrink");
    public void ButtonPress() => jsBridge.PlaySound("button-press");
    public void ChoiceCorrect() => jsBridge.PlaySound("choice-correct");
    public void ChoiceIncorrect() => jsBridge.PlaySound("choice-incorrect");
    public void ChoiceSelected() => jsBridge.PlaySound("choice-selected");
    public void Tada() => jsBridge.PlaySound("tada");
}