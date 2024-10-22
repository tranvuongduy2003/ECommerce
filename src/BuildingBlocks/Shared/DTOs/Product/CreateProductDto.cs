using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Product;

public class CreateProductDto : CreateOrUpdateProductDto
{
    [Required]
    [MaxLength(150, ErrorMessage = "Maximum length for Product No is 150 characters.")]
    public string No { get; set; }
}