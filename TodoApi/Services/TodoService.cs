using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly string _connectionString;

        public TodoService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("TodoDatabase") ?? "Data Source=todos.db";
            var builder = new SqliteConnectionStringBuilder(connectionString)
            {
                Pooling = false
            };
            _connectionString = builder.ToString();
        }

        public void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Todos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    IsCompleted INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL
                )";

            command.ExecuteNonQuery();
        }

        public Todo CreateTodo(Todo todo)
        {
            var createdAt = DateTime.UtcNow;

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Todos (Title, Description, IsCompleted, CreatedAt)
                VALUES (@title, @description, @isCompleted, @createdAt);
                SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@title", todo.Title);
            command.Parameters.AddWithValue("@description", (object?)todo.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@isCompleted", todo.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@createdAt", createdAt.ToString("o"));

            var id = Convert.ToInt32(command.ExecuteScalar());
            todo.Id = id;
            todo.CreatedAt = createdAt;
            return todo;
        }

        public IEnumerable<Todo> GetAllTodos()
        {
            var todos = new List<Todo>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                todos.Add(MapReader(reader));
            }

            return todos;
        }

        public Todo? GetTodoById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReader(reader);
            }

            return null;
        }

        public Todo? UpdateTodo(int id, Todo todo)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Todos
                SET Title = @title,
                    Description = @description,
                    IsCompleted = @isCompleted
                WHERE Id = @id";

            command.Parameters.AddWithValue("@title", todo.Title);
            command.Parameters.AddWithValue("@description", (object?)todo.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@isCompleted", todo.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0 ? GetTodoById(id) : null;
        }

        public bool DeleteTodo(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Todos WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            return command.ExecuteNonQuery() > 0;
        }

        private static Todo MapReader(SqliteDataReader reader)
        {
            return new Todo
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                IsCompleted = reader.GetInt32(3) == 1,
                CreatedAt = DateTime.Parse(reader.GetString(4))
            };
        }
    }
}
