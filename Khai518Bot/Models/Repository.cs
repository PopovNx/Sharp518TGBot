namespace Khai518Bot.Models;

[Serializable]
public class Repository
{
    public string Name { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public static Repository CreateInit(string name, string link, string role) => new Repository{
                                                                                    Name = name,
                                                                                    Link = link,
                                                                                    Role = role};
}