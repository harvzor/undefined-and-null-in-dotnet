using Harvzor.Optional;

namespace UpdateApi.Customer.Dtos.Input;

public class HarvzorOptionalCustomerPatchDto
{
    public Optional<string> Name { get; set; }
    
    public Optional<string?> Gender { get; set; }
    
    public Optional<bool> Deleted { get; set; }
}
