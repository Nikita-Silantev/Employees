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

    #region должность

    /// <summary>
    /// Создание объект должности
    /// </summary>
    /// <param name="post"></param>
    /// <returns></returns>
    public async Task<Post> CreatePost(Post post)
    {
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            const string sql = "insert into post (name, salary) values (@name, @salary) returning id, name, salary;";
            await using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("name", post.Name);
            command.Parameters.AddWithValue("salary", post.Salary);

            await using var reader = await command.ExecuteReaderAsync();
            Post response = new Post();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            response.Id = reader.GetInt32(reader.GetOrdinal("id"));
            response.Name = reader.GetString(reader.GetOrdinal("name"));
            response.Salary = reader.GetInt32(reader.GetOrdinal("salary"));
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Выдать все должности
    /// </summary>
    /// <returns></returns>
    public async Task<List<Post>> GetAllPosts()
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string sql = "SELECT * FROM post";

        await using var command = new NpgsqlCommand(sql, connection);

        await using var reader = command.ExecuteReader();


        List<Post> posts = new List<Post>();

        while (await reader.ReadAsync())
        {
            Post post = new Post();
            post.Id = reader.GetInt32(reader.GetOrdinal("id"));
            post.Name = reader.GetString(reader.GetOrdinal("name"));
            post.Salary = reader.GetInt32(reader.GetOrdinal("salary"));
            posts.Add(post);
        }

        return posts;
    }

    /// <summary>
    /// Обносить должность по Id
    /// </summary>
    /// <param name="post"></param>
    /// <returns></returns>
    public async Task<Post> UpdatePost(Post post)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string sql = "update post set name = @name, salary = @salary where id = @id returning id, name, salary;";

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", post.Id);
        command.Parameters.AddWithValue("name", post.Name);
        command.Parameters.AddWithValue("salary", post.Salary);

        await using var reader = await command.ExecuteReaderAsync();
        Console.WriteLine($"ID: {post.Id}");
        Console.WriteLine($"Name: {post.Name}");
        Console.WriteLine($"Salary: {post.Salary}");
        if (!await reader.ReadAsync())
        {
            return null;
        }


        var newPost = new Post
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Salary = reader.GetInt32(reader.GetOrdinal("salary")),
        };

        return newPost;
    }

    /// <summary>
    /// Удаление должности
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<string> DeletePost(int id)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string sql = "delete from post where id = @id;";

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
        return "OK";
    }

    #endregion

    #region задачи

    /// <summary>
    /// Создать задачу
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public async Task<EmpTask> CreateTask(EmpTask task)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string sql =
            "insert into task (name, date_start, date_end) values (@name, @date_start, @date_end) returning id, name, date_start, date_end;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("name", task.Name);
        command.Parameters.AddWithValue("date_start", task.Date_Started);
        command.Parameters.AddWithValue("date_end", task.Date_End);

        await using var reader = command.ExecuteReader();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        var response = new EmpTask
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Date_Started = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date_start")),
            Date_End = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date_end")),
        };
        return response;
    }

    /// <summary>
    /// Выдать все задачи
    /// </summary>
    /// <returns></returns>
    public async Task<List<EmpTask>> GetAllTasks()
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string sql = "SELECT * FROM task";
        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = command.ExecuteReader();

        List<EmpTask> tasks = new List<EmpTask>();

        while (await reader.ReadAsync())
        {
            var r = new EmpTask();
            r.Id = reader.GetInt32(reader.GetOrdinal("id"));
            r.Name = reader.GetString(reader.GetOrdinal("name"));
            r.Date_Started = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date_start"));
            r.Date_End = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date_end"));
            tasks.Add(r);
        }

        return tasks;
    }

    /// <summary>
    /// обновить задачу
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public async Task<EmpTask> UpdateTask(EmpTask task)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string sql =
            "update task set name = @name, date_start = @date_start, date_end = @date_end where id = @id returning id, name, date_start, date_end;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", task.Id);
        command.Parameters.AddWithValue("name", task.Name);
        command.Parameters.AddWithValue("date_start", task.Date_Started);
        command.Parameters.AddWithValue("date_end", task.Date_End);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        var response = new EmpTask
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Date_Started = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date_start")),
            Date_End = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date_end")),
        };
        return response;
    }

    /// <summary>
    /// удалить задачу
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<string> DeleteTask(int id)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string sql = "delete from task where id = @id;";

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);
        await command.ExecuteNonQueryAsync();
        return "OK";
    }

    #endregion
}