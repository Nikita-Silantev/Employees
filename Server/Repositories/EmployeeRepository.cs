using Npgsql;
using Server.Models;

namespace Server.Repositories;

public class EmployeeRepository
{
    /// <summary>
    /// Создать подключение
    /// </summary>
    /// <returns></returns>
    private NpgsqlConnection CreateConnection()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = "localhost",
            Port = 5432,
            Database = "employees",
            Username = "postgres",
            Password = "admin",
        };

        string connectionString = builder.ConnectionString;
        return new NpgsqlConnection(connectionString);
    }

    #region отделы

    /// <summary>
    /// создать отдел
    /// </summary>
    /// <param name="department"></param>
    /// <returns></returns>
    public async Task<Department?> CreateDepartment(Department department)
    {
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            const string sql = """
                               INSERT INTO department (name)
                               VALUES (@name)
                               RETURNING id, name
                               """;

            await using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("name", department.Name);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Department
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name"))
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return null;
        }
    }

    /// <summary>
    /// выдать все отделы
    /// </summary>
    /// <returns></returns>
    public async Task<List<Department?>> GetAllDepartment()
    {
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            const string sql = "SELECT * FROM department";

            await using var command = new NpgsqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            var departments = new List<Department>();
            while (await reader.ReadAsync())
            {
                departments.Add(new Department
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name"))
                });
            }

            return departments;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// обновить отдел по id 
    /// </summary>
    /// <param name="department"></param>
    /// <returns></returns>
    public async Task<Department> UpdateDepartment(Department department)
    {
        await using var connection = CreateConnection();

        await connection.OpenAsync();

        const string sql = "update department set name = @name where id = @id returning id, name;";
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("id", department.Id);
        command.Parameters.AddWithValue("name", department.Name);

        await using var rider = await command.ExecuteReaderAsync();
        await rider.ReadAsync();
        var new_department = new Department();

        new_department.Id = rider.GetInt32(rider.GetOrdinal("id"));
        new_department.Name = rider.GetString(rider.GetOrdinal("name"));

        return new_department;
    }

    /// <summary>
    /// удалить отдел по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<string> DeleteDepartment(int id)
    {
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            const string sql = "delete from department where id = @id;";

            await using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("id", id);

            await command.ExecuteNonQueryAsync();
            return "OK";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion
}