using Microsoft.AspNetCore.Mvc;
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
}
