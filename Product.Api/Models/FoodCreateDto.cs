using System.ComponentModel.DataAnnotations;

namespace Product.Api.Models
{
    public class FoodCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}
