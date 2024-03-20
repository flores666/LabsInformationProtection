using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Labs.Models;

public class InputModel
{
    [Required]
    [DisplayName("Ввод")]
    public string Input { get; set; }
    
    [DisplayName("Ключ")]
    public string? Key { get; set; }

    [DisplayName("Контейнер")]
    public string? Container { get; set; }
    
    [DisplayName("Вывод")]
    public string? Output { get; set; }
}