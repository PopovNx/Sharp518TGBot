using System.ComponentModel.DataAnnotations;

namespace Khai518Bot.Models;

public class LecturePair
{
    public LecturePair(int i) => Number = i;

    [UsedImplicitly]
    public LecturePair()
    {
    }

    public enum State
    {
        None,
        One,
        OneNominator,
        OneDenominator,
        Two
    }

    [Key] public int Id { get; set; }
    public Lecture? MainLecture { get; set; }
    public Lecture? SubLecture { get; set; }
    public State TypeState { get; set; }
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    public int Number { get; set; }
}