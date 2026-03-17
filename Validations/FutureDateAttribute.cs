using System.ComponentModel.DataAnnotations;

namespace Task_Management_App.Validations
{
    public class FutureDateAttribute: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateValue)
            {
                if (dateValue > DateTime.UtcNow)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("The date must be in the future.");
                }
            }
            return new ValidationResult("Invalid date format.");
        }
    }
}
