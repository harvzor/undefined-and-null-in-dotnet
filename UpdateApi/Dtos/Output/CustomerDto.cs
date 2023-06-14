namespace UpdateApi.Dtos.Output;

public class CustomerDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string? Gender { get; set; }
    
    public DateTimeOffset? DeletedDate { get; set; }
}
