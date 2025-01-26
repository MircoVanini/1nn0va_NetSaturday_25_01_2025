namespace NativeSceneAutomation.Endpoints;

using MediatR;
using Microsoft.AspNetCore.Http;
using NativeSceneAutomation.Command;
using NativeSceneAutomation.Endpoints.Dtos;

public static class NativitySceneEndpoints
{

    public static IEndpointRouteBuilder MapNativitySceneEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/nativity-scene/")
                             .WithOpenApi();

        group.MapPost("start-sunset", StartSunset);
        group.MapPost("start-sunrise", StartSunrise);
        group.MapPost("start-automatic", StartAutomaticExecution);

        return endpoints;
    }

    private static async Task<IResult> StartSunset(StartSunsetRequestDto args, IMediator _mediator)
    {
        var command = new StartSunset(args.Duration);
        await _mediator.Send(command);

        return Results.Ok();
    }

    private static async Task<IResult> StartSunrise(StartSunriseRequestDto args, IMediator _mediator)
    {
        var command = new StartSunrise(args.Duration);
        await _mediator.Send(command);

        return Results.Ok();
    }

    private static async Task<IResult> StartAutomaticExecution(StartAutomaticExecutionRequestDto args, IMediator _mediator)
    {
        var command = new StartAutomaticExecution(
                            args.SunriseDuration,
                            args.SunsetDuration,
                            args.DayDuration,
                            args.NightDuration);
        await _mediator.Send(command);

        return Results.Ok();
    }

}
