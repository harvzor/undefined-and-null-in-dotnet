using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DotNext;
using DotNext.Text.Json;

namespace UpdateApi.Dtos.Input.Patch;

public class DotNextOptionalCustomerPatchDto
{
    [JsonConverter(typeof(OptionalConverterFactory))]
    [Required]
    public Optional<string> Name { get; set; }
    
    [JsonConverter(typeof(OptionalConverterFactory))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string?> Gender { get; set; }
    
    [JsonConverter(typeof(OptionalConverterFactory))]
    public Optional<bool> Deleted { get; set; }
}
