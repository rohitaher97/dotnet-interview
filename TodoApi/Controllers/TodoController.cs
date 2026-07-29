using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/todos")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpPost]
        public IActionResult CreateTodo([FromBody] TodoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title is required.");
            }

            var todo = new Todo
            {
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted
            };

            var created = _todoService.CreateTodo(todo);
            return CreatedAtAction(nameof(GetTodoById), new { id = created.Id }, created);
        }

        [HttpGet]
        public IActionResult GetTodos()
        {
            var todos = _todoService.GetAllTodos();
            return Ok(todos);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetTodoById(int id)
        {
            var todo = _todoService.GetTodoById(id);
            return todo is null ? NotFound() : Ok(todo);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateTodo(int id, [FromBody] TodoUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title is required.");
            }

            var todo = new Todo
            {
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted
            };

            var updated = _todoService.UpdateTodo(id, todo);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteTodo(int id)
        {
            var deleted = _todoService.DeleteTodo(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
