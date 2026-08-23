namespace Server.Models;
/// <summary>
/// Задача (над которой работает пользователь) - Id, название, дата начала, дата конца
/// </summary>
public class Task
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly Date_Started { get; set; }
    public DateOnly Date_End { get; set; }
}