using TodoApi.Models;

namespace TodoApi.Services
{
    public interface ITodoService
    {
        void InitializeDatabase();
        Todo CreateTodo(Todo todo);
        IEnumerable<Todo> GetAllTodos();
        Todo? GetTodoById(int id);
        Todo? UpdateTodo(int id, Todo todo);
        bool DeleteTodo(int id);
    }
}
