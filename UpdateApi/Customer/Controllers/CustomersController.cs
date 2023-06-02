﻿using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Morcatko.AspNetCore.JsonMergePatch;
using UpdateApi.Customer.Dtos.Input;
using UpdateApi.Customer.Mappers;
using UpdateApi.Customer.Repositories;

namespace UpdateApi.Customer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
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
        var customer = _customersRepository.Find(id);

        if (customer == null)
            return NotFound();
        
        customer.Name = brokenCustomerPutDto.Name;
        customer.Gender = brokenCustomerPutDto.Gender;
        
        customer = _customersRepository.Update(customer);
        
        if (customer == null)
            return StatusCode(500);
        
        return Ok(customer.Map());
    }
    
    /// <summary>
    /// DotNext.Optional doesn't allow me to use `[Required]` to ensure not undefined.
    /// </summary>
    [HttpPut("dotnextoptional/{id:int}")]
    public IActionResult DotNextOptionalUpdateCustomer([FromRoute] int id, [FromBody] DotNextOptionalCustomerPutDto dotNextOptionalCustomerPutDto)
    {
        var customer = _customersRepository.Find(id);

        if (customer == null)
            return NotFound();
        
        customer.Name = dotNextOptionalCustomerPutDto.Name;
        
        if (dotNextOptionalCustomerPutDto.Gender.HasValue)
            customer.Gender = dotNextOptionalCustomerPutDto.Gender.OrDefault();
        
        customer = _customersRepository.Update(customer);
        
        if (customer == null)
            return StatusCode(500);
        
        return Ok(customer.Map());
    }
    
    /// <summary>
    /// Using Morcatko.AspNetCore.JsonMergePatch
    /// </summary>
    /// <remarks>
    /// Adding `.AddSystemTextJsonMergePatch();` causes Swagger to default to `application/merge-patch+json` so added `[Consumes(MediaTypeNames.Application.Json)]` to the class so everything else only accepts normal JSON.
    /// </remarks>
    [HttpPatch("morcatko/{id:int}")]
    [Consumes(JsonMergePatchDocument.ContentType)]
    public IActionResult MorcatkoPatch([FromRoute] int id, [FromBody] JsonMergePatchDocument<MorcatkoPatchCustomerDto> patch)
    {
        var customer = _customersRepository.Find(id);
        
        if (customer == null)
            return NotFound();
    
        customer = patch.ApplyToT(customer);
    
        customer = _customersRepository.Update(customer);
    
        if (customer == null)
            return StatusCode(500);
        
        return Ok(customer.Map());
    }
}
