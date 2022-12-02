using System.ComponentModel.DataAnnotations;

namespace Khai518Bot.Models;

public class Lecture
{
    [Key] public int Id { get; set; }
    public string? Title { get; set; }
    public string? Teacher { get; set; }
    public string? LinkPlatform { get; set; }
    public string? Link { get; set; }
}