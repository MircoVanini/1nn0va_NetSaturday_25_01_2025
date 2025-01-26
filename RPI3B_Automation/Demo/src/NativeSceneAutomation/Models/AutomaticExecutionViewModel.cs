namespace NativeSceneAutomation.Models;

using System.ComponentModel.DataAnnotations;

public class AutomaticExecutionViewModel
{
    [Required]
    [Range(0, 24 * 60 * 60)]
    public int SunriseDurationInSec { get; set; } = 10;
    [Required]
    [Range(0, 24 * 60 * 60)]
    public int SunsetDurationInSec { get; set; } = 10;
    [Required]
    [Range(0, 24 * 60 * 60)]
    public int DayDurationInSec { get; set; } = 30;
    [Required]
    [Range(0, 24 * 60 * 60)]
    public int NightDurationInSec { get; set; } = 10;
}
