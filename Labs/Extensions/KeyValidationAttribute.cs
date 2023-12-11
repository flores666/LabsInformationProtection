using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Labs.Extensions;

public class KeyValidationAttribute : ValidationAttribute
{
    private readonly string _regex;
    
    public KeyValidationAttribute(string regex)
    {
        _regex = regex;
    }
    
    public override bool IsValid(object? value)
    {
        if (value is not string input) return false;
        return Regex.IsMatch(input, _regex) && IsUniqueString(input);
    }

    private bool IsUniqueString(string input)
    {
        var distinctChars = input.Distinct().ToList();
        return input.SequenceEqual(distinctChars);
    }
}
