using System.ComponentModel.DataAnnotations;

namespace UpdateApi.Dtos.Input.Put;

public class BrokenCustomerPutDto
{
    [Required] // does work
    public string Name { get; set; }
    
    // https://stackoverflow.com/questions/71024060/distinguish-between-null-and-not-present-using-json-merge-patch-with-netcore-web
    // [Required] // doesn't work as it returns false if the property is null, but we want to allow explicit null values and avoid implicit null values
    public string? Gender { get; set; }
    
    // [Required] // doesn't work unless we also change the property to `bool?`, but then we have to handle nullability in our code (though it should never happen)
    public bool Deleted { get; set; }
}
