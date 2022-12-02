using System.ComponentModel.DataAnnotations;

namespace Khai518Bot.Models;

[Serializable]
public class Day
{
    [Key] public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<LecturePair> LecturePairs { get; set; } = new();

    public static Day CreateInit(int id, string name)
    {
        return new Day
        {
            Id = id,
            Name = name,
            LecturePairs = new List<LecturePair>
            {
                new(1), new(2), new(3), new(4)
            }
        };
    }
}