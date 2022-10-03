using Commerce.Core;
using Commerce.Core.Contracts;
using Commerce.Inventory.Service.Entities;
using MassTransit;

namespace Commerce.Inventory.Service.Consumers.ProductConsumers;

public class ProductCreatedConsumer : IConsumer<ProductCreated>
{
    // Product collection in Inventory DB(Different from actual Product Collection.)
    private readonly IRepository<Product> _productRepository;

    public ProductCreatedConsumer(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task Consume(ConsumeContext<ProductCreated> context)
    {
        var message = context.Message;
        var product = await _productRepository.GetAsync(message.productId);
        if (product != null) return;

        product = new Product
        {
            Id = message.productId,
            Name = message.Name,
            Description = message.Description
        };

        await _productRepository.CreateAsync(product);
    }
}