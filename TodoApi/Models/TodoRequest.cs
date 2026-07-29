namespace TodoApi.Models
{
    public class TodoRequest
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
