using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace General.Data.Types;

public class User : Entity
{
    [MaxLength(50)]
    public required string Username  { get; set; }
    
    [JsonIgnore]
    public string Password  { get; set; }
    
    [MaxLength(256)]
    public string? Name { get; set; }
}