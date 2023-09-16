using System.ComponentModel.DataAnnotations;
using Laba1.Extensions;

namespace Laba1.Models;

public class InputModel
{
    [Required]
    public string Input { get; set; }
    
    public string? Key { get; set; }
    
    public string? Output { get; set; }
}