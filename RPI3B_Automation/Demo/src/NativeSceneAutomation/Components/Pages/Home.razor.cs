namespace NativeSceneAutomation.Components.Pages;

using MediatR;
using NativeSceneAutomation.Command;
using NativeSceneAutomation.Models;

public partial class Home
{
    private readonly IMediator _mediator;

    public SunriseViewModel Sunrise { get; set; } = new();
    public SunsetViewModel Sunset { get; set; } = new();
    public AutomaticExecutionViewModel AutomaticExecution { get; set; } = new();


    public Home(IMediator mediator)
    {
        _mediator = mediator;
    }

    private async Task HandleStartSunrise()
    {
        var command = new StartSunrise(Sunrise.DurationInSec * 1000);
        await _mediator.Send(command);
    }

    private async Task HandleStartSunset()
    {
        var command = new StartSunset(Sunset.DurationInSec * 1000);
        await _mediator.Send(command);
    }

    private async Task HandleStartAuthomaticExeution()
    {
        var command = new StartAutomaticExecution(
                                    AutomaticExecution.SunriseDurationInSec * 1000,
                                    AutomaticExecution.SunsetDurationInSec       * 1000,
                                    AutomaticExecution.DayDurationInSec     * 1000,
                                    AutomaticExecution.NightDurationInSec   * 1000);
        await _mediator.Send(command);
    }



}
