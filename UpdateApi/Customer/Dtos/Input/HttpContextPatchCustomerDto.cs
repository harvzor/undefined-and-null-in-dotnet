﻿namespace UpdateApi.Customer.Dtos.Input;

public class HttpContextPatchCustomerDto
{
    public string Name { get; set; }
    
    public string? Gender { get; set; }
    
    public bool Deleted { get; set; }
}
