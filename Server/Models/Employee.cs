namespace Server.Models;
/// <summary>
/// Сотрудник
/// </summary>
public class Employee
{
    public int Id { get; set; }
    
    //Имя
    public string FirstName { get; set; }
    
    //Фамилия
    public string LastName { get; set; }
    
    //Отчество
    public string MiddleName { get; set; }
    
    //ДР
    public DateOnly Date_Birth { get; set; }
    
    //Пол
    public string Gender { get; set; }
    
    //В каком отделе
    public int Id_Department { get; set; }
    
    //Должность
    public int Id_Post { get; set; }
    
    //Ставка - 0.5 или 0.7 или 1.0
    public decimal Rate { get; set; }
    
    //Какой задачей занят - 0 = свободен делать 
    public int Id_Task { get; set; }
}