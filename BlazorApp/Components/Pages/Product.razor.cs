using BlazorApp.UnitOfWork;

namespace BlazorApp.Components.Pages
{
    public partial class Product
    {
        private List<ProductDto>? products;
        private string? errorMessage;
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            isLoading = true;
            errorMessage = null;
            StateHasChanged();

            var (items, error) = await ProductService.GetAllAsync();
            products = items;
            errorMessage = error;
            isLoading = false;
        }
    }
}
