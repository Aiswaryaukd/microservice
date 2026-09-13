using Microsoft.EntityFrameworkCore;
using Product.Application.Abstract;
using Product.Application.Default;
using Product.Domain.Abstract;
using Product.Infrastructure.Data;
using Product.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=app.db";

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.EnsureCreated();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new Product.Domain.Entities.ProductClass { Name = "Sample A", Price = 9.99m },
            new Product.Domain.Entities.ProductClass { Name = "Sample B", Price = 19.99m },
            new Product.Domain.Entities.ProductClass { Name = "Sample C", Price = 29.99m }
        );
    }

    if (!db.Messages.Any())
    {
        db.Messages.AddRange(
            new Product.Domain.Entities.MessageClass { Content = "Welcome to the message board." },
            new Product.Domain.Entities.MessageClass { Content = "Create a new message from the UI." }
        );
    }

    db.SaveChanges();
}

app.Run();
