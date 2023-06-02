namespace UpdateApi.Customer.Dtos.Input;

public class MorcatkoPatchCustomerDto
{
    public string Name { get; set; }
    
    public string? Gender { get; set; }
    
    public bool Deleted { get; set; }
}
