using System.ComponentModel.DataAnnotations;

namespace Khai518Bot.Models;

[Serializable]
public class ListenChat
{
    [Key] public int Id { get; set; }
    public long ChatId { get; set; }
}