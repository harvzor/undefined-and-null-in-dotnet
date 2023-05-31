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
    
    [HttpPut("{id:int}")]
    public IActionResult PutCustomer([FromRoute] int id, [FromBody] BrokenCustomerPutDto brokenCustomerPutDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            // Handle validation errors
            return BadRequest(errors);
        }
        
        var customer = _customersRepository.Update(id, brokenCustomerPutDto);

        if (customer == null)
            return NotFound();
        
        return Ok(customer.Map());
    }
}
