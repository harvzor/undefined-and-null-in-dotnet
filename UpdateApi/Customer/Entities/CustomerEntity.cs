namespace UpdateApi.Customer.Entities;

public class CustomerEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public string? Gender { get; set; }

    public CustomerEntity Clone()
    {
        return (CustomerEntity)MemberwiseClone();
    }
}