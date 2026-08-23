using Npgsql;
using Server.Models;

namespace Server.Repositories;

public class EmployeeRepository
{
    private NpgsqlConnection CreateCOnnection()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = "localhost",
            Port = 5432,
            Database = "Employees",
            Username = "postgres",
            Password = "postgres",
        };

        string connectionString = builder.ConnectionString;
        return new NpgsqlConnection(connectionString);
    }

    public async Task<Department?> CreateDepartment(Department department)
    {
        try
        {
            await using var connection = CreateCOnnection();
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
}