using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Shared.CustomDataAnnotations;

public sealed class MustContainASpecialCharacter : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string stringValue = value as string ?? string.Empty;
        
        return stringValue.Any(character => Char.IsLetterOrDigit(character) == false);
    }
}