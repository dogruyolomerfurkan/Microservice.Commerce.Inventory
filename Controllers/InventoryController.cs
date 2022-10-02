using Commerce.Core;
using Commerce.Inventory.Service.Clients;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Inventory.Service.Controllers;

[ApiController]
[Route("inventory")]
public class InventoryController : ControllerBase
{
    private readonly IRepository<Entities.Inventory> _inventoryRepository;
    private readonly ProductClient _productClient;
    public InventoryController(IRepository<Entities.Inventory> inventoryRepository, ProductClient productClient)
    {
        _inventoryRepository = inventoryRepository;
        _productClient = productClient;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty) return BadRequest();

        var inventoryItems = await _inventoryRepository.GetListAsync(item => item.UserId == userId);

        var productList = await _productClient.GetProductListAsync();

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