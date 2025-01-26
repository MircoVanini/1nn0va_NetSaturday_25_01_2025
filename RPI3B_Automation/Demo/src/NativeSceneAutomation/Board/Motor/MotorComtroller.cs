using System.Diagnostics;
using Iot.Device.Uln2003;

public class MotorController : IDisposable
{
    private Uln2003? _motor;

    public void Initialize()
    {
        _motor = new Uln2003(4, 18, 27, 22);

        _motor!.RPM = 15;
        _motor!.Mode = StepperMode.HalfStep;
    }

    public void SetSpeed(short rpm)
    {
        _motor!.RPM = rpm;
    }

    public Task Rotate180AntiClockwiseAsync(CancellationToken token)
    {
        return Task.Run(() =>
        {
            try
            {
                _motor?.Step(2048);
            }
            catch(TaskCanceledException)
            {
            }
        }, token);
    }

    public Task Rotate180ClockwiseAsync(CancellationToken token)
    {
        return Task.Run(() =>
        {
            try
            {
                _motor?.Step(-2048);
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    public Task RotateAntiClockwiseAsync(int duration, CancellationToken token)
    {
        return Task.Run(() => 
        {
            try
            { 
                Stopwatch sw = new Stopwatch();
                sw.Start();

                while(!token.IsCancellationRequested)
                {
                    _motor?.Step(2048);

                    if(sw.ElapsedMilliseconds >= duration)
                        break;  
                }

                sw.Stop();
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    public Task RotateAngleAntiClockwiseAsync(int angle, CancellationToken token)
    {
        return Task.Run(() => 
        {
            try
            { 
                int steps = (int)(2048.0 * angle / 180.0);

                _motor?.Step(steps);
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    public Task RotateAngleClockwiseAsync(int angle, CancellationToken token)
    {
        return Task.Run(() => 
        {
            try
            { 
                int steps = (int)(-2048.0 * angle / 180.0);

                _motor?.Step(steps);
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }


    public Task RotateClockwiseAsync(int duration, CancellationToken token)
    {
        return Task.Run(() => 
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                while(!token.IsCancellationRequested)
                {
                    _motor?.Step(-2048);
                    
                    if(sw.ElapsedMilliseconds >= duration)
                        break;  
                }

                sw.Stop();
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    public void Dispose()
    {
        _motor?.Dispose();
        _motor = null;
    }
}