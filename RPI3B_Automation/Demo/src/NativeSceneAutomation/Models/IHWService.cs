namespace NativeSceneAutomation.Models
{
    public interface IHWService
    {
        Task InitializeAsync();
        Task StartAsync();
        Task StopAsync();

        Task StartRotateAsync(int duration, bool clockwise);
        Task StartAngleRotateAsync(int angle);
        Task StopRotateAsync();

        Task TurnOffPixelsAsync();
        Task DayPixelsColorsTransitionAsync(int duration);
        Task NightPixelsColorsTransitionAsync(int duration);

    }
}
