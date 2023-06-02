using Microsoft.AspNetCore.Mvc;
using UpdateApi.Customer.Dtos.Input;
using UpdateApi.Customer.Mappers;
using UpdateApi.Customer.Repositories;

namespace UpdateApi.Customer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomersRepository _customersRepository;

    public CustomersController()
    {
        _customersRepository = new CustomersRepository();
    }
    
    [HttpGet]
    public IActionResult GetCustomers()
    {
        return Ok(
            _customersRepository
                .GetAll()
                .Map()
        );
    }
    
    [HttpGet("{id:int}")]
    public IActionResult GetCustomer([FromRoute] int id)
    {
        var customer = _customersRepository.Find(id);

        if (customer == null)
            return NotFound();
        
        return Ok(customer.Map());
    }
    
    /// <summary>
    /// Client can accidentally set `gender` to `null` by not sending the value (implicit null).
    /// </summary>
    [HttpPut("broken/{id:int}")]
    public IActionResult BrokenUpdateCustomer([FromRoute] int id, [FromBody] BrokenCustomerPutDto brokenCustomerPutDto)
    {
        var customer = _customersRepository.Update(id, brokenCustomerPutDto);

        if (customer == null)
            return NotFound();
        
        return Ok(customer.Map());
    }
    
    /// <summary>
    /// DotNext.Optional doesn't allow me to use `[Required]` to ensure not undefined.
    /// </summary>
    [HttpPut("dotnextoptional/{id:int}")]
    public IActionResult DotNextOptionalUpdateCustomer([FromRoute] int id, [FromBody] DotNextOptionalCustomerPutDto dotNextOptionalCustomerPutDto)
    {
        var customer = _customersRepository.Update(id, dotNextOptionalCustomerPutDto);

        if (customer == null)
            return NotFound();
        
        return Ok(customer.Map());
    }
}
