using System.ComponentModel.DataAnnotations;

namespace UpdateApi.Customer.Dtos.Input;

public class BrokenCustomerPutDto
{
    [Required]
    public string Name { get; set; }
    
    // https://stackoverflow.com/questions/71024060/distinguish-between-null-and-not-present-using-json-merge-patch-with-netcore-web
    // [Required] // doesn't work as it returns false if the property is null, but we want to allow null values.
    public string? Gender { get; set; }
}
