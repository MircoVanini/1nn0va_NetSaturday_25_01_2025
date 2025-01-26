namespace NativeSceneAutomation.Endpoints.Dtos;

public class StartAutomaticExecutionRequestDto
{
    public int SunriseDuration { get; set; }
    public int SunsetDuration { get; set; }
    public int DayDuration { get; set; }
    public int NightDuration { get; set; }
}
