using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Morcatko.AspNetCore.JsonMergePatch;
using UpdateApi.Attributes;
using UpdateApi.Dtos.Input.Patch;
using UpdateApi.Dtos.Input.Put;
using UpdateApi.Mappers;
using UpdateApi.Repositories;

namespace UpdateApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
public class CustomersController : ControllerBase
{
    private readonly CustomersRepository _customersRepository;
    private const string JsonPatchContentType = "application/json-patch+json";

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
    /// https://github.com/Havunen/SystemTextJsonPatch
    /// Preferable to using Microsoft.AspNetCore.JsonPatch.JsonPatchDocument as it works well with System.Text.Json.
    /// </summary>
    [HttpPatch("havunen/{id:int}")]
    [Consumes(JsonPatchContentType)]
    public IActionResult HavunenPatch([FromRoute] int id, [FromBody] SystemTextJsonPatch.JsonPatchDocument<BasicCustomerPatchDto> patch)
    {
        var customer = _customersRepository.Find(id);
        
        if (customer == null)
            return NotFound();
        
        // Cannot apply to different model.
        // customer = patch.ApplyTo(customer);
        
        // Example of how to read the operations:
        // All of the other properties would also have to be handled.
        var nameOperation = patch.Operations
            .Find(x => String.Equals(x.Path, "/" + nameof(BasicCustomerPatchDto.Name), StringComparison.OrdinalIgnoreCase));
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
    /// Using Microsoft.AspNetCore.JsonPatch
    /// https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-7.0
    /// This should work, but the Swagger docs will all be broken with no clear fix: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2094
    /// </summary>
    [HttpPatch("microsoft/{id:int}")]
    [Consumes(JsonPatchContentType)]
    public IActionResult MicrosoftPatch([FromRoute] int id, [FromBody] Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<BasicCustomerPatchDto> patch)
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
    public IActionResult MorcatkoPatch([FromRoute] int id, [FromBody] JsonMergePatchDocument<BasicCustomerPatchDto> patch)
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
            .Find(x => String.Equals(x.path, "/" + nameof(BasicCustomerPatchDto.Name), StringComparison.OrdinalIgnoreCase));
        if (nameOperation != null)
        {
            var name = (string)nameOperation.value;
            customer.Name = name;
        }
        
        var genderOperation = patch.Operations
            .Find(x => String.Equals(x.path, "/" + nameof(BasicCustomerPatchDto.Gender), StringComparison.OrdinalIgnoreCase));
        if (genderOperation != null)
        {
            var gender = (string?)genderOperation.value;
            customer.Gender = gender;
        }
    
        var deleteOperation = patch.Operations
            .Find(x => String.Equals(x.path, "/" + nameof(BasicCustomerPatchDto.Deleted), StringComparison.OrdinalIgnoreCase));
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
    [Consumes(JsonPatchContentType)]
    [ReadableBodyStream]
    public IActionResult HttpContextUpdateCustomer([FromRoute] int id, [FromBody] BasicCustomerPatchDto customerPatch)
    {
        var customer = _customersRepository.Find(id);
        
        if (customer == null)
            return NotFound();

        var bodyReader = new BodyReader(Request);

        if (bodyReader.HasProperty(nameof(BasicCustomerPatchDto.Name)))
            customer.Name = customerPatch.Name;
        
        if (bodyReader.HasProperty(nameof(BasicCustomerPatchDto.Gender)))
            customer.Gender = customerPatch.Gender;
        
        if (bodyReader.HasProperty(nameof(BasicCustomerPatchDto.Deleted)))
            customer.Delete(customerPatch.Deleted);

        customer = _customersRepository.Update(customer);
    
        if (customer == null)
            return StatusCode(500);
        
        return Ok(customer.Map());
    }
    
    /// <summary>
    /// DotNext.Optional doesn't work with nullable types (`T?`) as it says explicit `null` is not a real value.
    /// </summary>
    [HttpPatch("dotnextoptional/{id:int}")]
    // [Consumes("application/merge-patch+json")] // No idea why this causes an issue.
    [Consumes(JsonPatchContentType)]
    public IActionResult DotNextOptionalUpdateCustomer([FromRoute] int id, [FromBody] DotNextOptionalCustomerPatchDto dotNextOptionalCustomerPatchDto)
    {
        var customer = _customersRepository.Find(id);
    
        if (customer == null)
            return NotFound();
        
        if (dotNextOptionalCustomerPatchDto.Name.HasValue)
            customer.Name = dotNextOptionalCustomerPatchDto.Name.Value;
        
        // HasValue doesn't work on `T?` as it thinks that explicit `null` is not an intentional value.
        if (dotNextOptionalCustomerPatchDto.Gender.HasValue)
            customer.Gender = dotNextOptionalCustomerPatchDto.Gender.Value;
        
        if (dotNextOptionalCustomerPatchDto.Deleted.HasValue)
            customer.Delete(dotNextOptionalCustomerPatchDto.Deleted.Value);
        
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
    [Consumes(JsonPatchContentType)]
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
