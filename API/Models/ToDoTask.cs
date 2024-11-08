using System.ComponentModel.DataAnnotations;

namespace API.Models;

internal class FutureDateAttribute : ValidationAttribute
{
    // value needs to be an object? to override the default
    // otherwise it wouldve been a DateTime

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
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
    public int Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    [MaxLength(70)]
    public required string Title { get; set; }

    [FutureDate]
    public required DateTime? DueDate { get; set; }

    public required bool IsCompleted { get; set; }
}