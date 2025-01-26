namespace NativeSceneAutomation.Models;

using System.ComponentModel.DataAnnotations;

public class SunriseViewModel
{
    [Required]
    [Range(0, 24 * 60 * 60)]
    public int DurationInSec { get; set; } = 5;
}
