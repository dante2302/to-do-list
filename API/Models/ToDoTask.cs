using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class ToDoTask
{
    [Key]
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required DateTime DueDate { get; set; }
    public required bool IsCompleted { get; set; }
}