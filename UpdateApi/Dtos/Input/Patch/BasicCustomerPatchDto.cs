namespace UpdateApi.Dtos.Input.Patch;

public class BasicCustomerPatchDto
{
    public string Name { get; set; }
    
    public string? Gender { get; set; }
    
    public bool Deleted { get; set; }
}
