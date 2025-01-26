using System.Diagnostics;
using Iot.Device.Hcsr04;
using UnitsNet;

public class ProximityController : IDisposable
{
    private const short BounceLimit = 2;

    private Hcsr04? _sonar;
    private Task? _readTask;
    private bool _isOnRange = false;
    private volatile short _bounce = 0;

    public EventHandler<ProximityArgs>? ProximityChanged;

    public void Initialize()
    {
        _sonar = new(5, 6);
    }

    public Task StartReadAsync(CancellationToken token)
    {
        return _readTask = Task.Factory.StartNew(() =>
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (GetDistance() is double distance)
                    {
                        if (!_isOnRange && ++_bounce >= BounceLimit)
                            FireProximityChanged(true);
                    }
                    else
                    {
                        if (_isOnRange && ++_bounce >= BounceLimit)
                            FireProximityChanged(false);
                    }

                    Task.Delay(100).Wait();
                }
            }
            catch (TaskCanceledException)
            {
            }
        }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    private void FireProximityChanged(bool state)
    {
        _isOnRange = state;
        _bounce    = 0;

        ProximityChanged?.Invoke(this, new ProximityArgs { OnRange = state });
    }

    private double? GetDistance()
    {
        if (_sonar?.TryGetDistance(out Length distance) ?? false)
        {
            if (distance.Centimeters > 0 && distance.Centimeters < 15)
                return distance.Centimeters;
            else
                return null;
        }
        else
            return null;
    }

    public void Dispose()
    {
        _sonar?.Dispose();
        _sonar = null;
    }
}

public class ProximityArgs : EventArgs
{
    public bool OnRange { get; set; }
}