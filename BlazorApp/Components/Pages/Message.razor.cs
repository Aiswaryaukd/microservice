using BlazorApp.UnitOfWork;

namespace BlazorApp.Components.Pages
{
    public partial class Message
    {
        private List<MessageDto>? messages;
        private string? errorMessage;
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadMessagesAsync();
        }

        private async Task LoadMessagesAsync()
        {
            isLoading = true;
            errorMessage = null;
            StateHasChanged();

            var (items, error) = await MessageService.GetAllAsync();
            messages = items;
            errorMessage = error;
            isLoading = false;
        }
    }
}
