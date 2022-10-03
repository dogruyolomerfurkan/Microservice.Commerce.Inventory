using Commerce.Core;
using Commerce.Core.Contracts;
using Commerce.Inventory.Service.Entities;
using MassTransit;

namespace Commerce.Inventory.Service.Consumers.ProductConsumers;

public class ProductDeletedConsumer : IConsumer<ProductDeleted>
{
    private readonly IRepository<Product> _productRepository;

    public ProductDeletedConsumer(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task Consume(ConsumeContext<ProductDeleted> context)
    {
        var message = context.Message;
        var product = await _productRepository.GetAsync(message.productId);

        if (product == null) return;

        await _productRepository.RemoveAsync(message.productId);
    }
}