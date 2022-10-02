namespace Commerce.Inventory.Service;

public record GrantProductsDto(Guid UserId, Guid ProductId, int Quantity);
public record InventoryDto(Guid ProductId, string Name, string Description, int Quantity, DateTimeOffset AcquiredDate);
public record ProductDto(Guid Id, string Name, string Description);