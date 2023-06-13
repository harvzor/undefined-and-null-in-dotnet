﻿using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Morcatko.AspNetCore.JsonMergePatch;
using UpdateApi.Attributes;
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
    /// Issues:
    /// - client can accidentally set `gender` to `null` by not sending the value (implicit null)
    /// - client can accidentally undelete by not setting `delete` to `true` (implicit false)
    /// </summary>
    [HttpPut("broken/{id:int}")]
    public IActionResult BrokenUpdateCustomer([FromRoute] int id, [FromBody] BrokenCustomerPutDto brokenCustomerPutDto)
    {
        var customer = _customersRepository.Find(id);

        if (customer == null)
            return NotFound();
        
        customer.Name = brokenCustomerPutDto.Name;
        customer.Gender = brokenCustomerPutDto.Gender;
        customer.Delete(brokenCustomerPutDto.Deleted);
        
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
        
        customer.Delete(dotNextOptionalCustomerPutDto.Deleted);
        
        customer = _customersRepository.Update(customer);
        
        if (customer == null)
            return StatusCode(500);
        
        return Ok(customer.Map());
    }
    
    /// <summary>
    /// https://github.com/Havunen/SystemTextJsonPatch
    /// Preferable to using Microsoft.AspNetCore.JsonPatch.JsonPatchDocument as it works well with System.Text.Json.
    /// </summary>
    [HttpPatch("Havunen/{id:int}")]
    [Consumes("application/json-patch+json ")]
    public IActionResult HavunenPatch([FromRoute] int id, [FromBody] SystemTextJsonPatch.JsonPatchDocument<HavunenPatchCustomerDto> patch)
    {
        var customer = _customersRepository.Find(id);
        
        if (customer == null)
            return NotFound();
        
        // Cannot apply to different model.
        // customer = patch.ApplyTo(customer);
        
        // Example of how to read the operations:
        // All of the other properties would also have to be handled.
        var nameOperation = patch.Operations
            .Find(x => String.Equals(x.Path, "/" + nameof(MorcatkoPatchCustomerDto.Name), StringComparison.OrdinalIgnoreCase));
        if (nameOperation != null)
        {
            var name = (string)nameOperation.Value;
            customer.Name = name;
        }
        
        customer = _customersRepository.Update(customer);
    
        if (customer == null)
            return StatusCode(500);
        
        return Ok(customer.Map());
    }
    
    /// <summary>
    /// https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-7.0
    /// This should work, but the Swagger docs will all be broken with no clear fix: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2094
    /// </summary>
    [HttpPatch("{id:int}")]
    [Consumes("application/json-patch+json ")]
    public IActionResult Patch([FromRoute] int id, [FromBody] Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<MicrosoftPatchCustomerDto> patch)
    {
        var customer = _customersRepository.Find(id);
        
        if (customer == null)
            return NotFound();
        
        // Cannot apply to different model.
        // customer = patch.ApplyTo(customer);
        
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
    
        // Can't use if my DTO doesn't map to the target 100%.
        // Deleted does not map to DeletedDate.
        // customer = patch.ApplyToT(customer);
        
        // This is JSON Merge PATCH so I don't need to look out for operations that aren't matching Replace.
        // I lose out on nice strong typing:
        var nameOperation = patch.Operations
            .Find(x => String.Equals(x.path, "/" + nameof(MorcatkoPatchCustomerDto.Name), StringComparison.OrdinalIgnoreCase));
        if (nameOperation != null)
        {
            var name = (string)nameOperation.value;
            customer.Name = name;
        }
        
        var genderOperation = patch.Operations
            .Find(x => String.Equals(x.path, "/" + nameof(MorcatkoPatchCustomerDto.Gender), StringComparison.OrdinalIgnoreCase));
        if (genderOperation != null)
        {
            var gender = (string?)genderOperation.value;
            customer.Gender = gender;
        }
    
        var deleteOperation = patch.Operations
            .Find(x => String.Equals(x.path, "/" + nameof(MorcatkoPatchCustomerDto.Deleted), StringComparison.OrdinalIgnoreCase));
        if (deleteOperation != null)
        {
            var deleted = (bool)deleteOperation.value;
            customer.Delete(deleted);
        }
        
        customer = _customersRepository.Update(customer);
    
        if (customer == null)
            return StatusCode(500);
        
        return Ok(customer.Map());
    }

    private class BodyReader
    {
        private readonly JsonDocument _jsonDocument;
        
        public BodyReader(HttpRequest httpRequest)
        {
            httpRequest.Body.Seek(0, SeekOrigin.Begin);
            
            using var reader = new StreamReader(httpRequest.Body);
            
            var jsonPayload = reader.ReadToEnd();

            _jsonDocument = JsonDocument.Parse(jsonPayload);
        }

        public bool HasProperty(string propertyName)
        {
            var property = _jsonDocument.RootElement
                .EnumerateObject()
                .FirstOrDefault(p => string.Compare(p.Name, propertyName, 
                    StringComparison.OrdinalIgnoreCase) == 0);

            return property.Value.ValueKind != JsonValueKind.Undefined;

            // Cannot handle casing:
            // return _jsonDocument.RootElement.TryGetProperty(propertyName, out _);
        }
    }
    
    /// <summary>
    /// Checking the raw JSON in the HttpContext.Request.
    /// </summary>
    [HttpPatch("httpcontext/{id:int}")]
    // [Consumes("application/merge-patch+json")] // No idea why this causes an issue.
    [Consumes(MediaTypeNames.Application.Json)]
    [ReadableBodyStream]
    public IActionResult HttpContextUpdateCustomer([FromRoute] int id, [FromBody] HttpContextPatchCustomerDto patch)
    {
        var customer = _customersRepository.Find(id);
        
        if (customer == null)
            return NotFound();

        var bodyReader = new BodyReader(Request);

        if (bodyReader.HasProperty(nameof(HttpContextPatchCustomerDto.Name)))
            customer.Name = patch.Name;
        
        if (bodyReader.HasProperty(nameof(HttpContextPatchCustomerDto.Gender)))
            customer.Gender = patch.Gender;
        
        if (bodyReader.HasProperty(nameof(HttpContextPatchCustomerDto.Deleted)))
            customer.Delete(patch.Deleted);

        customer = _customersRepository.Update(customer);
    
        if (customer == null)
            return StatusCode(500);
        
        return Ok(customer.Map());
    }
    
    /// <summary>
    /// Using Harvzor.Optional
    /// </summary>
    [HttpPatch("harvzor-optional/{id:int}")]
    // [Consumes("application/merge-patch+json")] // No idea why this causes an issue.
    [Consumes(MediaTypeNames.Application.Json)]
    public IActionResult HarvzorOptionalUpdateCustomer([FromRoute] int id, [FromBody] HarvzorOptionalCustomerPatchDto patch)
    {
        var customer = _customersRepository.Find(id);
        
        if (customer == null)
            return NotFound();

        if (patch.Name.IsDefined)
            customer.Name = patch.Name.Value;
        
        if (patch.Gender.IsDefined)
            customer.Gender = patch.Gender.Value;
        
        if (patch.Deleted.IsDefined)
            customer.Delete(patch.Deleted.Value);

        customer = _customersRepository.Update(customer);
    
        if (customer == null)
            return StatusCode(500);
        
        return Ok(customer.Map());
    }
}
