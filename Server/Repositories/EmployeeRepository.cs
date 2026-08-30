using Npgsql;
using Server.Models;

namespace Server.Repositories;

public class EmployeeRepository
{
    private readonly IConfiguration _configuration;
    
    public EmployeeRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    /// <summary>
    /// Создать подключение
    /// </summary>
    /// <returns></returns>
    private NpgsqlConnection CreateConnection()
    {
        var connectionString =
            _configuration.GetConnectionString("EmployeesDatabase");

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

    #region сотрудники

    /// <summary>
    /// Создание сотрудника
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    public async Task<Employee> CreateEmployee(Employee employee)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string sql =
            "insert into employee (f_name, l_name, m_name, date_birth, id_department, id_post, id_task, rate) values (@f_name, @l_name, @m_name, @date_birth, @id_department, @id_post, @id_task, @rate) returning id, f_name, l_name, m_name, date_birth, id_department, id_post, id_task, rate;";

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("id", employee.Id);
        command.Parameters.AddWithValue("f_name", employee.FirstName);
        command.Parameters.AddWithValue("l_name", employee.LastName);
        command.Parameters.AddWithValue("m_name", employee.MiddleName);
        command.Parameters.AddWithValue("date_birth", employee.Date_Birth);
        command.Parameters.AddWithValue("id_department", employee.Id_Department);
        command.Parameters.AddWithValue("id_post", employee.Id_Post);
        command.Parameters.AddWithValue("id_task", employee.Id_Task);
        command.Parameters.AddWithValue("rate", employee.Rate);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        var createdEmployee = new Employee
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            FirstName = reader.GetString(reader.GetOrdinal("f_name")),
            MiddleName = reader.GetString(reader.GetOrdinal("l_name")),
            LastName = reader.GetString(reader.GetOrdinal("m_name")),
            Date_Birth = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date_birth")),
            Id_Department = reader.GetInt32(reader.GetOrdinal("id_department")),
            Id_Post = reader.GetInt32(reader.GetOrdinal("id_post")),
            Id_Task = reader.GetInt32(reader.GetOrdinal("id_task")),
            Rate = reader.GetDecimal(reader.GetOrdinal("rate")),
        };

        return createdEmployee;
    }

    /// <summary>
    /// проверка существования отдела
    /// </summary>
    /// <param name="id_department"></param>
    /// <returns></returns>
    public async Task<bool> CheckDepartment(int id_department)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        const string sql = "select id from department;";

        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        List<int> ids = new List<int>();

        while (await reader.ReadAsync())
        {
            int id = reader.GetInt32(reader.GetOrdinal("id"));
            ids.Add(id);
        }

        foreach (var id in ids)
        {
            if (id == id_department)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// проверка существования должноси
    /// </summary>
    /// <param name="id_post"></param>
    /// <returns></returns>
    public async Task<bool> CheckPost(int id_post)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        const string sql = "select id from post;";

        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        List<int> ids = new List<int>();

        while (await reader.ReadAsync())
        {
            int id = reader.GetInt32(reader.GetOrdinal("id"));
            ids.Add(id);
        }

        foreach (var id in ids)
        {
            if (id == id_post)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// проверка существования задачи
    /// </summary>
    /// <param name="id_task"></param>
    /// <returns></returns>
    public async Task<bool> CheckTask(int id_task)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        const string sql = "select id from task;";

        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        List<int> ids = new List<int>();

        while (await reader.ReadAsync())
        {
            int id = reader.GetInt32(reader.GetOrdinal("id"));
            ids.Add(id);
        }

        foreach (var id in ids)
        {
            if (id == id_task)
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}