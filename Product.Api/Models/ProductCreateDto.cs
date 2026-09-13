using System.ComponentModel.DataAnnotations;

namespace Product.Api.Models
{
    public class ProductCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
