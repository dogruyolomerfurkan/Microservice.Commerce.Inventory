using Commerce.Core;
using Commerce.Core.Contracts;
using Commerce.Inventory.Service.Entities;
using MassTransit;

namespace Commerce.Inventory.Service.Consumers.ProductConsumers;

public class ProductUpdatedConsumer : IConsumer<ProductUpdated>
{
    private readonly IRepository<Product> _productRepository;

    public ProductUpdatedConsumer(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task Consume(ConsumeContext<ProductUpdated> context)
    {
        var message = context.Message;
        var product = await _productRepository.GetAsync(message.productId);

        //Create product if it's null
        if (product is null)
        {
            product = new Product
            {
                Id = message.productId,
                Name = message.Name,
                Description = message.Description
            };

            await _productRepository.CreateAsync(product);
        }
        else
        {
            product.Name = message.Name;
            product.Description = message.Description;

            await _productRepository.UpdateAsync(product);
        }
    }
}