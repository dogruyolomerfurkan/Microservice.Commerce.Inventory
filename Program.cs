using Commerce.Core.MassTransit;
using Commerce.Core.MongoDB;
using Commerce.Inventory.Service.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongoDB()
                .AddMongoDBRepository<Inventory>("Inventory")
                .AddMongoDBRepository<Product>("Product")
                .AddMassTransitWithRabbitMq();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
