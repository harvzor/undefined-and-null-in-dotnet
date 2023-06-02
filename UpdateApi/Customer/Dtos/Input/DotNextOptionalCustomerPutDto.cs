using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DotNext;
using DotNext.Text.Json;

namespace UpdateApi.Customer.Dtos.Input;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class OptionalRequiredAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // if (value == null)
        // {
        //     return new ValidationResult(ErrorMessage);
        // }

        return ValidationResult.Success;
    }
}

public class DotNextOptionalCustomerPutDto
{
    [Required]
    public string Name { get; set; }
    
    [JsonConverter(typeof(OptionalConverterFactory))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    // [Required]
    // [OptionalRequired]
    public Optional<string?> Gender { get; set; }
    
    // [Required] // Required doesn't work unless I make it `bool?`, but then I have to handle it being null in my code, even though it can't be.
    public bool Deleted { get; set; }
}
