using System;

namespace ClientEmp.Models;

public class EmployeeDictionaryItem
{
    public int Id { get; set; }
    
    //Имя
    public string FirstName { get; set; }
    
    //Фамилия
    public string LastName { get; set; }
    
    //Отчество
    public string MiddleName { get; set; }
    
    //ДР
    public DateTime? Date_Birth { get; set; }
    
    //В каком отделе
    public string Department { get; set; }
    
    //Должность
    public string Post { get; set; }
    
    //Ставка - 0.5 или 0.7 или 1.0
    public decimal Rate { get; set; }
    
    //Какой задачей занят - 0 = свободен делать 
    public string Task { get; set; }
}