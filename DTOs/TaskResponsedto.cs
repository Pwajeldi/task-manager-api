namespace Task_Management_App.DTOs
{
    public class TaskResponsedto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}
