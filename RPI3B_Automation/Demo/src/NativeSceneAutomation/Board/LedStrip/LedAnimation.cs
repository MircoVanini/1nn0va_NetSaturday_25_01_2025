using System;
using System.Drawing;
using Iot.Device.BrickPi3.Sensors;
using Iot.Device.Graphics;
using Iot.Device.PiJuiceDevice.Models;
using Iot.Device.Ws28xx;
using Color = System.Drawing.Color;

namespace NativeSceneAutomation.Board.LedStrip;

public class LedAnimations
{
    private int _ledCount;
    private Iot.Device.Ws28xx.Ws28xx _ledStrip;

    private int[] _dayLED   = [ 255, 255, 164 ];
    private int[] _nightLED = [ 0,    32,  50 ];
    private int[] _sunLED   = [ 255, 157,  64 ];

    private int[] _gamma8 =
    [
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  1,  1,  1,  1,
        1,  1,  1,  1,  1,  1,  1,  1,  1,  2,  2,  2,  2,  2,  2,  2,
        2,  3,  3,  3,  3,  3,  3,  3,  4,  4,  4,  4,  4,  5,  5,  5,
        5,  6,  6,  6,  6,  7,  7,  7,  7,  8,  8,  8,  9,  9,  9, 10,
        10, 10, 11, 11, 11, 12, 12, 13, 13, 13, 14, 14, 15, 15, 16, 16,
        17, 17, 18, 18, 19, 19, 20, 20, 21, 21, 22, 22, 23, 24, 24, 25,
        25, 26, 27, 27, 28, 29, 29, 30, 31, 32, 32, 33, 34, 35, 35, 36,
        37, 38, 39, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 50,
        51, 52, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 66, 67, 68,
        69, 70, 72, 73, 74, 75, 77, 78, 79, 81, 82, 83, 85, 86, 87, 89,
        90, 92, 93, 95, 96, 98, 99, 101, 102, 104, 105, 107, 109, 110, 112, 114,
        115, 117, 119, 120, 122, 124, 126, 127, 129, 131, 133, 135, 137, 138, 140, 142,
        144, 146, 148, 150, 152, 154, 196, 158, 160, 162, 164, 167, 169, 171, 173, 175,
        177, 180, 182, 184, 186, 189, 191, 193, 196, 198, 200, 203, 205, 208, 210, 213,
        215, 218, 220, 223, 225, 228, 231, 233, 236, 239, 241, 244, 247, 249, 252, 255
    ];

    public LedAnimations(Iot.Device.Ws28xx.Ws28xx ledStrip, int ledCount)
    {
        this._ledStrip = ledStrip;
        this._ledCount = ledCount;
    }

    public virtual bool SupportsSeparateWhite { get; set; } = false;

    public void ColorWipe(Color color)
    {
        var img = _ledStrip.Image;
        for (var i = 0; i < _ledCount; i++)
        {
            img.SetPixel(i, 0, color);
            _ledStrip.Update();
            Thread.Sleep(25);
        }
    }

    public Color FilterColor(Color source)
    {
        return SupportsSeparateWhite ? Color.FromArgb(0, source.R, source.G, source.B) : source;
    }

    public async void KnightRider(CancellationToken token)
    {
        var img = _ledStrip.Image;
        var downDirection = false;

        var beamLength = 15;

        var index = 0;
        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < _ledCount; i++)
            {
                img.SetPixel(i, 0, Color.FromArgb(0, 0, 0, 0));
            }

            if (downDirection)
            {
                for (int i = 0; i <= beamLength; i++)
                {
                    if (index + i < _ledCount && index + i >= 0)
                    {
                        var redValue = (beamLength - i) * (255 / (beamLength + 1));
                        img.SetPixel(index + i, 0, Color.FromArgb(0, redValue, 0, 0));
                    }
                }

                index--;
                if (index < -beamLength)
                {
                    downDirection = false;
                    index = 0;
                }
            }
            else
            {
                for (int i = beamLength - 1; i >= 0; i--)
                {
                    if (index - i >= 0 && index - i < _ledCount)
                    {
                        var redValue = (beamLength - i) * (255 / (beamLength + 1));
                        img.SetPixel(index - i, 0, Color.FromArgb(0, redValue, 0, 0));
                    }
                }

                index++;
                if (index - beamLength >= _ledCount)
                {
                    downDirection = true;
                    index = _ledCount - 1;
                }
            }

            _ledStrip.Update();
            await Task.Delay(10).ConfigureAwait(false);
        }
    }

    public void NightPixelsColors(int duration, CancellationToken token)
    {
        // https://www.reddit.com/r/arduino/comments/9zyiie/i_made_an_arduino_aquarium_light_with/?rdt=49277

        RawPixelContainer img = _ledStrip.Image;
            
        int red   = 0;
        int green = 0;
        int blue  = 0;
        int delay = duration / 60 / 2;

        // SUNSET
        //
        for (int i = 0; i < 60; i++)
        {
            if (token.IsCancellationRequested)
                break;

            red   = Map(i, 0, 59, _dayLED[0], _sunLED[0]);
            green = Map(i, 0, 59, _dayLED[1], _sunLED[1]);
            blue  = Map(i, 0, 59, _dayLED[2], _sunLED[2]);

            red   = _gamma8[red];
            green = _gamma8[green];
            blue  = _gamma8[blue];

            for (var j = 0; j < _ledCount; j++)
            {
                if (token.IsCancellationRequested)
                    break;

                img.SetPixel(j, 0, Color.FromArgb(red, green, blue));
            }

            _ledStrip.Update();

            Thread.Sleep(delay);
        }

        // DUSK
        //
        for (int i = 0; i < 60; i++)
        {
            if (token.IsCancellationRequested)
                break;

            red   = Map(i, 0, 59, _sunLED[0], _nightLED[0]);
            green = Map(i, 0, 59, _sunLED[1], _nightLED[1]);
            blue  = Map(i, 0, 59, _sunLED[2], _nightLED[2]);

            red   = _gamma8[red];
            green = _gamma8[green];
            blue  = _gamma8[blue];

            for (var j = 0; j < _ledCount; j++)
            {
                if (token.IsCancellationRequested)
                    break;

                img.SetPixel(j, 0, Color.FromArgb(red, green, blue));
            }

            _ledStrip.Update();

            Thread.Sleep(delay);
        }

    }   

    private void SetColorToPixels(Color color)
    {
        RawPixelContainer img = _ledStrip.Image;
        for (var j = 0; j < _ledCount; j++)
            img.SetPixel(j, 0, color);
        _ledStrip.Update();
    }

    public void DayPixelsColors(int duration, CancellationToken token)
    {
        // https://www.reddit.com/r/arduino/comments/9zyiie/i_made_an_arduino_aquarium_light_with/?rdt=49277

        RawPixelContainer img = _ledStrip.Image;
            
        int red   = 0;
        int green = 0;
        int blue  = 0;
        int delay = duration / 60 / 2;

        // SUNRISE
        //
        for (int i = 0; i < 60; i++)
        {
            if (token.IsCancellationRequested)
                break;

            red   = Map(i, 0, 59, _nightLED[0], _sunLED[0]);
            green = Map(i, 0, 59, _nightLED[1], _sunLED[1]);
            blue  = Map(i, 0, 59, _nightLED[2], _sunLED[2]);

            red   = _gamma8[red];
            green = _gamma8[green];
            blue  = _gamma8[blue];

            for (var j = 0; j < _ledCount; j++)
            {
                if (token.IsCancellationRequested)
                    break;

                img.SetPixel(j, 0, Color.FromArgb(red, green, blue));
            }

            _ledStrip.Update();

            Thread.Sleep(delay);
        }

        // DAWN
        //
        for (int i = 0; i < 60; i++)
        {
            if (token.IsCancellationRequested)
                break;

            red   = Map(i, 0, 59, _sunLED[0], _dayLED[0]);
            green = Map(i, 0, 59, _sunLED[1], _dayLED[1]);
            blue  = Map(i, 0, 59, _sunLED[2], _dayLED[2]);

            red = _gamma8[red];
            green = _gamma8[green];
            blue = _gamma8[blue];

            for (var j = 0; j < _ledCount; j++)
            {
                if (token.IsCancellationRequested)
                    break;

                img.SetPixel(j, 0, Color.FromArgb(red, green, blue));
            }

            _ledStrip.Update();

            Thread.Sleep(delay);
        }
    }

    /// <summary>
    /// Rainbows the specified count.
    /// </summary>
    /// <param name="token">The token.</param>
    public void Rainbow(CancellationToken token)
    {
        RawPixelContainer img = _ledStrip.Image;
        while (!token.IsCancellationRequested)
        {
            for (var i = 0; i < 255; i++)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                for (var j = 0; j < _ledCount; j++)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    img.SetPixel(j, 0, Wheel((i + j) & 255));
                }

                _ledStrip.Update();
                Thread.Sleep(25);
            }
        }
    }

    /// <summary>
    /// Sets the color of the entire strip.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="count">The count.</param>
    public void SetColor(Color color, int count)
    {
        RawPixelContainer img = _ledStrip.Image;
        for (var i = 0; i < count; i++)
        {
            img.SetPixel(i, 0, color);
        }

        _ledStrip.Update();
    }

    /// <summary>
    /// Sets the white value using a percentag.
    /// </summary>
    /// <param name="colorPercentage">The color percentage.</param>
    /// <param name="separateWhite">if set to <c>true</c> [separate white].</param>
    public void SetWhiteValue(float colorPercentage, bool separateWhite = false)
    {
        var color = Color.FromArgb(separateWhite ? (int)(255 * colorPercentage) : 0, !separateWhite ? (int)(255 * colorPercentage) : 0, !separateWhite ? (int)(255 * colorPercentage) : 0, !separateWhite ? (int)(255 * colorPercentage) : 0);
        SetColor(color, _ledCount);
    }

    /// <summary>
    /// Switches the LEDs off.
    /// </summary>
    public void SwitchOffLeds()
    {
        var img = _ledStrip.Image;
        img.Clear();
        _ledStrip.Update();
    }

    /// <summary>
    /// Theatre Chase animation.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="blankColor">Color of the blank.</param>
    /// <param name="token">The token.</param>
    public void TheatreChase(Color color, Color blankColor, CancellationToken token)
    {
        RawPixelContainer img = _ledStrip.Image;
        while (!token.IsCancellationRequested)
        {
            for (var j = 0; j < 3; j++)
            {
                for (var k = 0; k < _ledCount; k += 3)
                {
                    img.SetPixel(j + k, 0, color);
                }

                _ledStrip.Update();
                Thread.Sleep(100);

                for (var k = 0; k < _ledCount; k += 3)
                {
                    img.SetPixel(j + k, 0, blankColor);
                }
            }
        }
    }

    private Color Wheel(int position)
    {
        if (position < 85)
        {
            return Color.FromArgb(0, position * 3, 255 - position * 3, 0);
        }
        else if (position < 170)
        {
            position -= 85;
            return Color.FromArgb(0, 255 - position * 3, 0, position * 3);
        }
        else
        {
            position -= 170;
            return Color.FromArgb(0, 0, position * 3, 255 - position * 3);
        }
    }

    private int Map(int value, int fromLow, int fromHigh, int toLow, int toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow; 
    }
}