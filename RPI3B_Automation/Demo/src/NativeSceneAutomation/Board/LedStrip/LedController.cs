using System;
using System.Device.Spi;
using System.Drawing;
using Iot.Device.Graphics;
using Iot.Device.Ws28xx;

namespace NativeSceneAutomation.Board.LedStrip;

public class LedController : IDisposable
{
    private SpiDevice? _spi;
    private LedAnimations? _leds;

    public void Initialize()
    {
        SpiConnectionSettings settings = new (0, 0)
        {
            ClockFrequency = 2_400_000,
            Mode = SpiMode.Mode0,
            DataBitLength = 8
        };

        _spi  = SpiDevice.Create(settings);
        _leds = new LedAnimations(new Ws2812b(_spi, 60), 60);
        _leds!.SwitchOffLeds();
    }

    public Task StartRaimbowAsync(CancellationToken token)
    {
        return Task.Run(async () =>
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    _leds!.Rainbow(token);
                    await Task.Delay(100, token);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }, 
        token);
    }

    public Task StartDayPixelsColorsAsync(int duration, CancellationToken token)
    {
        return Task.Run(() =>
        {
            try
            {
                _leds!.DayPixelsColors(duration, token);
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    public Task StartNightPixelsColorsAsync(int duration, CancellationToken token)
    {
        return Task.Run(() =>
        {
            try
            {
                _leds!.NightPixelsColors(duration, token);
            }
            catch (TaskCanceledException)
            {
            }   
        }, token);
    }
    public Task StartSwitchOffLedsAsync()
    {
        return Task.Run(() => _leds!.SwitchOffLeds());
    }

    public void SetWhiteValue(byte value)
    {
        _leds!.SetWhiteValue(value, true);
    }

    public void SetColorWipe(Color color)
    {
        _leds!.ColorWipe(_leds!.FilterColor(color));
    }

    public Task StartChaseAsync(Color color, Color blankColor, CancellationToken token)
    {
        return Task.Run(() =>
        {
            try
            {
                _leds!.TheatreChase(color, blankColor, token);
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    public Task StartKnightRiderAsync(CancellationToken token)
    {
        return Task.Run(() =>
        {
            try
            {
                _leds!.KnightRider(token);
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    public void Dispose()
    {
        _spi?.Dispose();
        _spi = null;
    }
}
