using System.ComponentModel.DataAnnotations;

namespace General.Data.Types;

public class Todo : Entity
{
    [MaxLength(256)]
    public string? Name  { get; set; }
    
    public bool IsComplete { get; set; }
}