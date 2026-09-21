using System;

namespace ClientEmp.Models;
/// <summary>
/// Задача (над которой работает пользователь) - Id, название, дата начала, дата конца
/// </summary>
public class EmpTask
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime? Date_Started { get; set; }
    public DateTime? Date_End { get; set; }
}