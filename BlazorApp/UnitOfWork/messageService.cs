using System.ComponentModel.DataAnnotations;

namespace BlazorApp.UnitOfWork
{
    public class MessageClientService
    {
        private readonly HttpClient _http;

        public MessageClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<(List<MessageDto>? Messages, string? Error)> GetAllAsync()
        {
            try
            {
                var messages = await _http.GetFromJsonAsync<List<MessageDto>>("api/message");
                return (messages ?? new List<MessageDto>(), null);
            }
            catch (HttpRequestException)
            {
                return (null, "Cannot connect to the API. Make sure Product.Api and APIGateway are running.");
            }
            catch (Exception)
            {
                return (null, "Failed to load messages. Please try again.");
            }
        }

        public async Task<(MessageDto? Message, string? Error)> CreateAsync(MessageCreateModel model)
        {
            try
            {
                var payload = new { model.Content };
                var response = await _http.PostAsJsonAsync("api/message", payload);

                if (response.IsSuccessStatusCode)
                {
                    var created = await response.Content.ReadFromJsonAsync<MessageDto>();
                    return (created, null);
                }

                return (null, $"Create failed ({(int)response.StatusCode}). Check the message and try again.");
            }
            catch (HttpRequestException)
            {
                return (null, "Cannot connect to the API. Make sure Product.Api and APIGateway are running.");
            }
            catch (Exception)
            {
                return (null, "Failed to create message. Please try again.");
            }
        }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class MessageCreateModel
    {
        [Required(ErrorMessage = "Message content is required.")]
        [StringLength(500, ErrorMessage = "Content cannot exceed 500 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}
