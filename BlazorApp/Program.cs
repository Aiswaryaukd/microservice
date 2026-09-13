using BlazorApp.Components;
using BlazorApp.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("Gateway", client =>
{
    client.BaseAddress = new Uri("http://localhost:5083/");
});

builder.Services.AddScoped(sp =>
    new ProductService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("Gateway")));
builder.Services.AddScoped(sp =>
    new MessageClientService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("Gateway")));
builder.Services.AddScoped(sp =>
    new FoodClientService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("Gateway")));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
