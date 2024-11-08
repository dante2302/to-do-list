using System.ComponentModel.DataAnnotations;

namespace API.Models;

internal class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (value is DateTime date && date < DateTime.Today)
        {
            return new ValidationResult("The date cannot be in the past.");
        }

        return ValidationResult.Success;
    }
}

public class ToDoTask
{
    [Key]
    public required int Id { get; set; }

    [MaxLength(70)]
    public required string Title { get; set; }

    [FutureDate]
    public required DateTime? DueDate { get; set; }

    public required bool IsCompleted { get; set; }
}