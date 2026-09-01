namespace Server.Models;
/// <summary>
/// Должность - Id, название, зарплата
/// </summary>
public class Post
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Salary { get; set; }
}