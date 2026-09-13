using System.ComponentModel.DataAnnotations;

namespace BlazorApp.UnitOfWork
{
    public class ProductService
    {
        private readonly HttpClient _http;

        public ProductService(HttpClient http)
        {
            _http = http;
        }

        public async Task<(List<ProductDto>? Products, string? Error)> GetAllAsync()
        {
            try
            {
                var products = await _http.GetFromJsonAsync<List<ProductDto>>("api/product");
                return (products ?? new List<ProductDto>(), null);
            }
            catch (HttpRequestException)
            {
                return (null, "Cannot connect to the API. Make sure Product.Api and APIGateway are running.");
            }
            catch (Exception)
            {
                return (null, "Failed to load products. Please try again.");
            }
        }

        public async Task<(ProductDto? Product, string? Error)> CreateAsync(ProductCreateModel model)
        {
            try
            {
                var payload = new { model.Name, model.Price };
                var response = await _http.PostAsJsonAsync("api/product", payload);

                if (response.IsSuccessStatusCode)
                {
                    var created = await response.Content.ReadFromJsonAsync<ProductDto>();
                    return (created, null);
                }

                return (null, $"Create failed ({(int)response.StatusCode}). Check the product details and try again.");
            }
            catch (HttpRequestException)
            {
                return (null, "Cannot connect to the API. Make sure Product.Api and APIGateway are running.");
            }
            catch (Exception)
            {
                return (null, "Failed to create product. Please try again.");
            }
        }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class ProductCreateModel
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99.")]
        public decimal? Price { get; set; }
    }
}
