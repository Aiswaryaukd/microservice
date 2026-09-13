using System.ComponentModel.DataAnnotations;

namespace BlazorApp.UnitOfWork
{
    public class FoodClientService
    {
        private readonly HttpClient _http;

        public FoodClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<(List<FoodDto>? Foods, string? Error)> GetAllAsync()
        {
            try
            {
                var foods = await _http.GetFromJsonAsync<List<FoodDto>>("api/food");
                return (foods ?? new List<FoodDto>(), null);
            }
            catch (HttpRequestException)
            {
                return (null, "Cannot connect to the API. Make sure Product.Api and APIGateway are running.");
            }
            catch (Exception)
            {
                return (null, "Failed to load foods. Please try again.");
            }
        }

        public async Task<(FoodDto? Food, string? Error)> CreateAsync(FoodCreateModel model)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/food", new { model.Name });
                if (response.IsSuccessStatusCode)
                {
                    var created = await response.Content.ReadFromJsonAsync<FoodDto>();
                    return (created, null);
                }

                return (null, $"Create failed ({(int)response.StatusCode}). Check the food name and try again.");
            }
            catch (HttpRequestException)
            {
                return (null, "Cannot connect to the API. Make sure Product.Api and APIGateway are running.");
            }
            catch (Exception)
            {
                return (null, "Failed to create food. Please try again.");
            }
        }
    }

    public class FoodDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class FoodCreateModel
    {
        [Required(ErrorMessage = "Food name is required.")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}
