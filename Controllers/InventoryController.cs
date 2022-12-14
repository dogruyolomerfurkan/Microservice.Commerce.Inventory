using Commerce.Core;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Inventory.Service.Controllers;

[ApiController]
[Route("inventory")]
public class InventoryController : ControllerBase
{
    private readonly IRepository<Entities.Inventory> _inventoryRepository;
    private readonly IRepository<Entities.Product> _productRepository;

    public InventoryController(IRepository<Entities.Inventory> inventoryRepository, IRepository<Entities.Product> productRepository)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty) return BadRequest();

        var inventoryItems = await _inventoryRepository.GetListAsync(item => item.UserId == userId);
        var productIds = inventoryItems.Select(p => p.ProductId);

        var productList = await _productRepository.GetListAsync(product => productIds.Contains(product.Id));

        var inventoryList = inventoryItems.Select(inventoryItem =>
        {
            var product = productList.Single(product => product.Id == inventoryItem.ProductId);
            return inventoryItem.AdaptDto(product.Name, product.Description);
        });
        return Ok(inventoryList);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantProductsDto grantProductsDto)
    {
        var inventoryItem = await _inventoryRepository.GetAsync(item => item.UserId == grantProductsDto.UserId
        && item.ProductId == grantProductsDto.ProductId);

        if (inventoryItem is null)
        {
            inventoryItem = new Entities.Inventory
            {
                ProductId = grantProductsDto.ProductId,
                UserId = grantProductsDto.UserId,
                Quantity = grantProductsDto.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };

            await _inventoryRepository.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grantProductsDto.Quantity;
            await _inventoryRepository.UpdateAsync(inventoryItem);
        }
        return Ok();
    }
}