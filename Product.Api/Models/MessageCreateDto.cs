using System.ComponentModel.DataAnnotations;

namespace Product.Api.Models
{
    public class MessageCreateDto
    {
        [Required(ErrorMessage = "Message content is required.")]
        [StringLength(500, ErrorMessage = "Content cannot exceed 500 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}
