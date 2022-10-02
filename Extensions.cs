namespace Commerce.Inventory.Service;

public static class Extensions
{
    public static InventoryDto AdaptDto(this Entities.Inventory inventory, string name, string description)
    {
        return new InventoryDto(inventory.Id, name, description, inventory.Quantity, inventory.AcquiredDate);
    }
}