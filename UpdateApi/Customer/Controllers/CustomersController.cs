using Microsoft.AspNetCore.Mvc;
using UpdateApi.Customer.Dtos;

namespace UpdateApi.Customer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetCustomers()
    {
        return Ok(new CustomerDto[]
        {
            new()
            {
                Id = 1,
                Name = "Harvey",
            },
        });
    }
    
    [HttpGet("{id:int}")]
    public IActionResult GetCustomer([FromRoute] int id)
    {
        return Ok(new CustomerDto
        {
            Id = id,
            Name = "Harvey",
        });
    }
}
